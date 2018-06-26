//20170829 modi by shenbao for P001-170829002 指标树函数信息支持多语言
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common.Torridity.Metadata;
using Digiwin.Common.Torridity;
using System.ComponentModel;
using Digiwin.ERP.Common.Utils;
using Digiwin.Common;
using Digiwin.Common.UI;
using Digiwin.Common.Services;
using System.Text.RegularExpressions;
using Digiwin.Common.Configuration;

namespace Digiwin.ERP.TFL.UI.Implement {
    public class UIEntity {
        public static List<FunctionEntity> CreateViewEntity(DependencyObjectCollection collFunc,ServiceComponent service) {
            var group = collFunc.GroupBy(c => new { FUNCTION_INFO_NAME = c["FUNCTION_INFO_NAME"].ToStringExtension(),
                                                    FUNCTION_INFO_DISPLAY_NAME = c["FUNCTION_INFO_DISPLAY_NAME"].ToStringExtension(),
                                                    FUNCTION_INFO_CLASS = c["FUNCTION_INFO_CLASS"].ToStringExtension(),
                                                    FUNCTION_INFO_CLASS_NAME = c["FUNCTION_INFO_CLASS_NAME"].ToStringExtension(),
                                                    FUNCTION_EXPLAIN = c["FUNCTION_EXPLAIN"].ToStringExtension()
            });
            List<FunctionEntity> list = new List<FunctionEntity>();
            IQueryProjectContainer srv = service.GetService<IQueryProjectContainer>("TFL");
            IMetadataContainer mc = service.GetService<IMetadataContainer>();
            Regex regNum = new Regex("^[0-9]");  //参数不能以数字开头的校验

            foreach (var item in group) {
                FunctionEntity entity = new FunctionEntity() { Name = item.Key.FUNCTION_INFO_NAME, DisplayName = item.Key.FUNCTION_INFO_DISPLAY_NAME, Description = item.Key.FUNCTION_EXPLAIN, Category = item.Key.FUNCTION_INFO_CLASS_NAME };
                foreach (var sub in item) {
                    if (Maths.IsEmpty(sub["FUNCTION_INFO_D_ID"]))  //没有参数
                        continue;
                    if(regNum.IsMatch(sub["PARAMETER_CODE"].ToStringExtension()))  //参数不能以数字开头
                        continue;

                    ParaEntity para = new ParaEntity();
                    para.Name = sub["PARAMETER_CODE"].ToStringExtension();
                    para.DisplayName = sub["PARAMETER_NAME"].ToStringExtension();
                    para.OffSet = sub["PLACEHOLDER"].ToStringExtension();
                    int index = para.Name.IndexOf(".");
                    if (index > -1) {
                        para.PropertyName = para.Name.Replace('.', '_');  //带上表明前缀 以防止有相同的属性名参数但是表明不一样（a.Status,b.Status），导致后续跟进参数注册属性会报错，不能注册相同的参数
                    } else {   //可能存在查询单一表情况，不带别名的
                        para.PropertyName = para.Name;
                    }
                    para.SecondPropertyName = para.PropertyName + "2";

                    para.ControlType = (ControlTypeEnum)sub["CONTROL_TYPE"].ToInt32();  //控件类型 1.录入框  2.开窗 3.下拉框 4.日期框
                    para.ParaTypeFlag = (ParaTypeEnum)sub["PARAMETER_VALUE_TYPE"].ToInt32(); //参数值类型 1.String 2.Integer 3.Double 4.Boolean 5.Date 6.DateTime
                    if (Maths.IsNotEmpty(sub["ORGANIZATION_TYPE"].ToStringExtension())) {
                        para.OrgType = (OrgnizationType)(Enum.Parse(typeof(OrgnizationType), sub["ORGANIZATION_TYPE"].ToStringExtension()));  //组织类型 1.公司 2.销售域 3.采购域 4.服务域  5.工厂 6.部门 7.工作中心
                    } else {
                        para.OrgType = OrgnizationType.Empty;
                    }
                    para.QueryTypeKey = sub["WINDOW_TYPEKEY"].ToStringExtension();  //开窗typekey

                    OperParaProName(0, para, sub, mc, srv);
                    int count = GetTheSameCount(entity, para.PropertyName);
                    if (count > 0) {
                        OperParaProName(count, para, sub, mc, srv);
                    }

                    para.ConditionName = "Condition_" + para.PropertyName;
                    para.Conditions = sub["CONDITIONS"].ToStringExtension();
                    if ((para.ControlType == ControlTypeEnum.SelectControl && para.OrgType == OrgnizationType.COMPANY)) {//组织类型为公司的开窗参数才需要添加当前条件项
                        if (Maths.IsNotEmpty(para.Conditions)) {
                            para.Conditions = para.Conditions.Insert(0, "Current;");
                        } else {
                            para.Conditions = "Current";
                        }
                    }
                    if ((para.ParaTypeFlag == ParaTypeEnum.String && para.ControlType == ControlTypeEnum.Input)) {//录入框类的string类型，要添加引用单元格条件项
                        if (Maths.IsNotEmpty(para.Conditions)) {
                            para.Conditions += ";ExcelCell";
                        } else {
                            para.Conditions = "ExcelCell";
                        }
                    }
                    string[] conditions = para.Conditions.Split(';');
                    if (conditions.Length > 0 && Maths.IsNotEmpty(conditions[0])) {
                        para.Operator = GetConditionCompareEnumValue(conditions[0]);
                    }

                    ParaEntity tmpP = entity.ParaList.FirstOrDefault(c => c.PropertyName == para.PropertyName);
                    if (tmpP == null) {  //不能有相同的参数
                        entity.ParaList.Add(para);
                    }

                }

                list.Add(entity);
            }

            return list;
        }

        private static int GetTheSameCount(FunctionEntity entity, string propertyName) {
            int count = 0;
            string tmpString = propertyName;
            if (propertyName.EndsWith("_ID")) {
                tmpString = propertyName.Substring(0, propertyName.Length - 3);
            } else if (propertyName.EndsWith("_CODE") || propertyName.EndsWith("_NAME")) {
                tmpString = propertyName.Substring(0, propertyName.Length - 5);
            }
            Regex regValidate = new Regex(@"^" + tmpString + @"X?\d{0,}$");
            foreach (var para in entity.ParaList) {
                tmpString = para.PropertyName;
                if (para.PropertyName.Contains("_ID")) {
                    tmpString = para.PropertyName.Replace("_ID", "");
                } else if (para.PropertyName.Contains("_CODE")) {
                    tmpString = para.PropertyName.Replace("_CODE", "");
                } else if (para.PropertyName.Contains("_NAME")) {
                    tmpString = para.PropertyName.Replace("_NAME", "");
                }
                if (regValidate.IsMatch(tmpString)) {
                    count++;
                }
            }

            return count;
        }

        private static void OperParaProName(int existCount, ParaEntity para, DependencyObject sub, IMetadataContainer mc, IQueryProjectContainer srv) {
            string name = para.Name;
            string extendFix = string.Empty;
            if (existCount > 0) {
                extendFix = "X" + (existCount + 1);  //后续以X+流水
                name = name + extendFix;
            }
            int index = name.IndexOf(".");
            if (index > -1) {
                para.PropertyName = name.Replace('.', '_');  //带上表明前缀 以防止有相同的属性名参数但是表明不一样（a.Status,b.Status），导致后续跟进参数注册属性会报错，不能注册相同的参数
            } else {   //可能存在查询单一表情况，不带别名的
                para.PropertyName = name;
            }
            para.SecondPropertyName = para.PropertyName + "2";
            if (para.ControlType == ControlTypeEnum.PickList) {
                para.PickListTypeName = sub["CONTROL_CODE"].ToStringExtension();
                if (para.PickListTypeName != string.Empty) {
                    var pick = mc.GetPickListTypeElement(para.PickListTypeName);
                    if (pick != null) {
                        //para.Tip = pick.DisplayName; //20170829 mark by shenbao for P001-170829002
                        LocalizableString localStr = new LocalizableString(pick.DisplayName); //20170829 add by shenbao for P001-170829002
                        para.Tip = localStr[System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterWindowsLanguageName]; //20170829 add by shenbao for P001-170829002
                    }
                }
            } else if (para.ControlType == ControlTypeEnum.SelectControl) {
                para.QueryProjectId = sub["CONTROL_CODE"].ToStringExtension();
                QueryProject qp = srv.QueryProjects.FirstOrDefault(c => c.Id == para.QueryProjectId) as QueryProject;
                string idColumn = string.Empty;
                if (qp != null) {
                    idColumn = qp.ContextDataColumn;
                    para.Tip = qp.DisplayName;
                } else {
                    idColumn = "KEY_FIELD_ID";  //理论上永远不会走到这里，除非FINCTION_INFO没有配置开窗编号，或者这个编号是错误的
                }
                para.TargetEntityPirmaryKeyID = idColumn;
                if (index >= 0) {
                    para.PropertyName = name.Substring(0, index) + "_" + idColumn + extendFix;  //如果是开窗类型，则需要重新设置PropertyName，为ID字段
                } else {
                    para.PropertyName = idColumn + extendFix;
                }
                para.SecondPropertyName = para.PropertyName + "2";

            }
        }

        private static OperatorTypeEnum GetConditionCompareEnumValue(string para) {
            switch (para) {
                case "Current":
                    return OperatorTypeEnum.Current;
                case "=":
                    return OperatorTypeEnum.Equal;
                case "!=":
                    return OperatorTypeEnum.NotEqual;
                case ">":
                    return OperatorTypeEnum.GreaterThan;
                case ">=":
                    return OperatorTypeEnum.GreaterEqual;
                case "<":
                    return OperatorTypeEnum.LessThan;
                case "<=":
                    return OperatorTypeEnum.LessEqual;
                case "Between":
                    return OperatorTypeEnum.Between;
                case "In":
                    return OperatorTypeEnum.In;
                case "Like":
                    return OperatorTypeEnum.Like;
                case "ExcelCell":
                    return OperatorTypeEnum.ExcelCell;
            }

            return OperatorTypeEnum.Equal;
        }

    }

    /// <summary>
    /// 函数
    /// </summary>
    public class FunctionEntity {
        public FunctionEntity() {
            _paraList = new List<ParaEntity>();
        }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category {
            get;
            set;
        }

        /// <summary>
        /// 是否财务公式
        /// </summary>
        public bool IsFinance {
            get;
            set;
        }

        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// 函数显示名称
        /// </summary>
        public string DisplayName {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description {
            get;
            set;
        }

        /// <summary>
        /// 公式结果
        /// </summary>
        public string FormulaResult {
            get;
            set;
        }

        List<ParaEntity> _paraList = null;
        /// <summary>
        /// 参数集合
        /// </summary>
        public List<ParaEntity> ParaList {
            get {
                return _paraList;
            }
            set {
                _paraList = value;
            }
        }

        public string CalculteFormulaResult() {
            string formulaResult = string.Empty;
            foreach (ParaEntity para in this._paraList) {
                if (para.ParaValueList.Count <= 0)
                    continue;
                if (formulaResult == string.Empty)
                    formulaResult = GetLambda(para);
                else
                    formulaResult += GetLambda(para);
            }

            formulaResult = @"TFL(""" + this.Name + @""",""" + formulaResult + @""","""")";

            return formulaResult;
        }

        private string GetLambda(ParaEntity para) {
            string pre = "[" + para.OffSet + ":";
            string value = string.Empty;

            switch (para.Operator) {
                case OperatorTypeEnum.Equal:
                case OperatorTypeEnum.ExcelCell:
                case OperatorTypeEnum.Current:
                    value = GetValue(para, "=");
                    break;
                case OperatorTypeEnum.GreaterEqual:
                    value = GetValue(para, ">=");
                    break;
                case OperatorTypeEnum.GreaterThan:
                    value = GetValue(para, ">");
                    break;
                case OperatorTypeEnum.LessEqual:
                    value = GetValue(para, "<=");
                    break;
                case OperatorTypeEnum.LessThan:
                    value = GetValue(para, "<");
                    break;
                case OperatorTypeEnum.NotEqual:
                    value = GetValue(para, "!=");
                    break;
                case OperatorTypeEnum.In:
                    value = GetValue(para, "in");
                    break;
                case OperatorTypeEnum.Between:
                    value = GetValue(para, "Between");
                    break;
                case OperatorTypeEnum.Like:
                    value = GetValue(para, "like");
                    break;
            }

            if (value != string.Empty) {
                value += "]";
                return pre += value;
            }
            return string.Empty;
        }

        private string GetValue(ParaEntity para, string oper) {
            string value = string.Empty;
            bool strFlag = (para.ParaTypeFlag == ParaTypeEnum.String || para.ParaTypeFlag == ParaTypeEnum.DateTime || para.ParaTypeFlag == ParaTypeEnum.Date);
            if (oper == "in") {
                foreach (ValueEntity item in para.ParaValueList) {
                    if (Maths.IsEmpty(item.Value))
                        continue;
                    if (strFlag) {
                        string tmpValue = ReOperValue(item.Value.ToStringExtension());
                        if (value == string.Empty)
                            value = "'" + tmpValue + "'";
                        else
                            value += "," + "'" + tmpValue + "'";
                    } else {
                        if (value == string.Empty)
                            value = item.Value.ToStringExtension();
                        else
                            value += "," + item.Value;
                    }
                }

                if (value != string.Empty) {
                    value = para.Name + " in (" + value + ")";
                }
            } else if (oper == "Between") {
                if (Maths.IsNotEmpty(para.ParaValueList[0].Value) && Maths.IsNotEmpty(para.ParaValueList[1].Value)) {
                    object tmpValue1 = para.ParaValueList[0].Value;
                    object tmpValue2 = para.ParaValueList[1].Value;
                    if (para.ControlType == ControlTypeEnum.SelectControl) {
                        tmpValue1 = para.ParaValueList[0].ReturnValue;
                        tmpValue2 = para.ParaValueList[1].ReturnValue;
                    }
                    //if (para.ParaTypeFlag == ParaTypeEnum.Date || para.ParaTypeFlag == ParaTypeEnum.DateTime) {
                    //    tmpValue1 = tmpValue1.ToDate().ToString("yyyy-MM-dd");
                    //    tmpValue2 = tmpValue2.ToDate().ToString("yyyy-MM-dd");
                    //}
                    if (strFlag) {
                        tmpValue1 = ReOperValue(tmpValue1.ToStringExtension());
                        tmpValue2 = ReOperValue(tmpValue2.ToStringExtension());
                        value = para.Name + ">='" + tmpValue1 + "' and " + para.Name + " <= '" + tmpValue2 + "'";
                    } else {
                        value = para.Name + ">=" + tmpValue1 + " and " + para.Name + " <= " + tmpValue2;
                    }
                }
            } else {
                if (Maths.IsNotEmpty(para.ParaValueList[0].Value)) {
                    object tmpValue = para.ParaValueList[0].Value;
                    if (para.ControlType == ControlTypeEnum.SelectControl) {
                        tmpValue = para.ParaValueList[0].ReturnValue;  //如果是开窗，则取固定的返回值ReturnValue
                    }
                    //if (para.ParaTypeFlag == ParaTypeEnum.Date || para.ParaTypeFlag == ParaTypeEnum.DateTime) {
                    //    tmpValue = tmpValue.ToDate().ToString("yyyy-MM-dd");
                    //}
                    if (oper == "like") {
                        tmpValue = ReOperValue(tmpValue.ToStringExtension());
                        value = para.Name + " " + oper + " '%" + tmpValue + "%'";
                    } else {
                        if (strFlag) {
                            if (para.Operator == OperatorTypeEnum.ExcelCell) {
                                value = para.Name + oper + @"'""&" + tmpValue + @"&""'";
                            } else {
                                tmpValue = ReOperValue(tmpValue.ToStringExtension());
                                value = para.Name + oper + "'" + tmpValue + "'";
                            }

                        } else {
                            value = para.Name + oper + tmpValue;
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 值转义
        /// </summary>
        /// <returns></returns>
        private string ReOperValue(string value) {
            if (value == null)
                return string.Empty;

            //单引号的转义
            value = value.Replace("'", "''");

            return value;
        }
    }

    /// <summary>
    /// 参数类型
    /// </summary>
    public class ParaEntity {
        public ParaEntity() {
            _paraValueList = new List<ValueEntity>();
            this.Operator = OperatorTypeEnum.Equal;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// 绑定实体的名称
        /// 由于Name可能是a.STATUS,鉴于额E10在绑定时对小数点符号的限制，没有办法直接用Name绑定
        /// 且考虑到一个sql语句中可能存在多个表的STATUS条件，比如还有个条件b.STATUS
        /// 所以采用[表明_属性名]作为实际的绑定字段 
        /// 比如a_STATUS,b_STATUS等
        /// </summary>
        public string PropertyName {
            get;
            set;
        }

        /// <summary>
        /// 由于sql语句中针对单号比如a.DOC_NO，这个类型可能是开窗，这个时间就需要知道这个开窗的RetrunField是什么，一般是主键字段
        /// </summary>
        public string TargetEntityPirmaryKeyID {
            get;
            set;
        }

        /// <summary>
        /// 对于between类型可能需要第二个参数
        /// </summary>
        public string SecondPropertyName {
            get;
            set;
        }

        /// <summary>
        /// 参数显示名称
        /// </summary>
        public string DisplayName {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description {
            get;
            set;
        }

        /// <summary>
        /// 参数对应的条件名称
        /// 每个参数对应一个条件参数（=、>）等，默认名称Condition_+参数名称
        /// </summary>
        public string ConditionName {
            get;
            set;
        }

        /// <summary>
        /// 条件
        /// </summary>
        public string Conditions {
            get;
            set;
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        public ControlTypeEnum ControlType {
            get;
            set;
        }

        /// <summary>
        /// 参数值类型对应的枚举
        /// </summary>
        public ParaTypeEnum ParaTypeFlag {
            get;
            set;
        }

        /// <summary>
        /// 参数的组织类型
        /// </summary>
        public OrgnizationType OrgType {
            get;
            set;
        }

        Type _type = null;
        /// <summary>
        /// 该类型作为后续注册数据源类型的依据
        /// </summary>
        public Type ParaType {
            get {
                if (this.ControlType == ControlTypeEnum.SelectControl) {  //开窗比较特殊，用ControlType判断
                    _type = typeof(Guid);
                } else if (this.ParaTypeFlag == ParaTypeEnum.String) {
                    _type = typeof(string);
                } else if (this.ParaTypeFlag == ParaTypeEnum.DateTime||this.ParaTypeFlag== ParaTypeEnum.Date) {
                    _type = typeof(DateTime);
                } else if (this.ParaTypeFlag == ParaTypeEnum.Decimal) {
                    _type = typeof(decimal);
                } else if (this.ParaTypeFlag == ParaTypeEnum.Boolean) {
                    _type = typeof(string);  //bool暂时采用picklist模式
                } else {
                    _type = typeof(string);
                }
                return _type;
            }
        }

        private OperatorTypeEnum _operator = OperatorTypeEnum.Equal;
        /// <summary>
        /// 对应的操作符
        /// </summary>
        public OperatorTypeEnum Operator {
            get {
                return _operator;
            }
            set {
                _operator = value;
                this.ParaValueList.Clear();
                this.ParaValueList.Add(new ValueEntity());
                if (this.Operator == OperatorTypeEnum.Between) {  //如果是between控件，则默认创建2个值
                    this.ParaValueList.Add(new ValueEntity());
                }
            }
        }

        /// <summary>
        /// 参数占位符
        /// </summary>
        public string OffSet {
            get;
            set;
        }

        /// <summary>
        /// 开窗类型时需要的查询方案所属TypeKey
        /// </summary>
        public string QueryTypeKey {
            get;
            set;
        }

        /// <summary>
        /// 开窗类型时需要的查询方案ID
        /// </summary>
        public string QueryProjectId {
            get;
            set;
        }

        /// <summary>
        /// 开窗类型时的tip
        /// </summary>
        public string Tip {
            get;
            set;
        }

        /// <summary>
        /// PickList类型时对应的picklist名称
        /// </summary>
        public string PickListTypeName {
            get;
            set;
        }

        /// <summary>
        /// 参数所属行
        /// </summary>
        public int Row {
            get;
            set;
        }

        /// <summary>
        /// 参数所属列
        /// </summary>
        public int Column {
            get;
            set;
        }

        private List<ValueEntity> _paraValueList = null;
        /// <summary>
        /// 参数值集合
        /// </summary>
        public List<ValueEntity> ParaValueList {
            get {
                return _paraValueList;
            }
            set {
                _paraValueList = value;
            }
        }

        /// <summary>
        /// 参数默认值
        /// </summary>
        public object DefaultValue {
            get {
                if (this.ControlType == ControlTypeEnum.SelectControl)
                    return Maths.GuidDefaultValue();
                else if (this.ControlType == ControlTypeEnum.PickList)
                    return string.Empty;
                else if (this.ParaTypeFlag == ParaTypeEnum.Boolean)
                    return "";  //bool暂时采用picklist模式
                else if (this.ParaTypeFlag == ParaTypeEnum.DateTime || this.ParaTypeFlag == ParaTypeEnum.Date)
                    return OrmDataOption.EmptyDateTime;
                else if (this.ParaTypeFlag == ParaTypeEnum.Decimal)
                    return 0;
                else
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// 参数值对象
    /// </summary>
    public class ValueEntity {
        /// <summary>
        /// 值
        /// </summary>
        public object Value {
            get;
            set;
        }

        [DefaultValue("")]
        /// <summary>
        /// 开窗控件暂存code
        /// </summary>
        public string HiddenText {
            get;
            set;
        }

        [DefaultValue("")]
        /// <summary>
        /// 开窗控件暂存name
        /// </summary>
        public string DisplayText {
            get;
            set;
        }

        /// <summary>
        /// 开窗特殊值
        /// 抓取返回值
        /// </summary>
        public object ReturnValue {
            get;
            set;
        }
    }

    /// <summary>
    /// 参数类别
    /// </summary>
    public enum ControlTypeEnum {
        /// <summary>
        /// 下拉框
        /// </summary>
        PickList = 3,
        /// <summary>
        /// 开窗
        /// </summary>
        SelectControl = 2,
        /// <summary>
        /// 录入框
        /// </summary>
        Input = 1,
        /// <summary>
        /// 日期框
        /// </summary>
        DateControl = 4
    }

    public enum ParaTypeEnum {
        /// <summary>
        /// 日期
        /// </summary>
        DateTime = 6,
        /// <summary>
        /// 数字
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 1,
        /// <summary>
        /// bool类型
        /// </summary>
        Boolean = 4,
        /// <summary>
        /// 整数
        /// </summary>
        Integer = 2,
        /// <summary>
        /// 日期
        /// </summary>
        Date = 5,
    }

    /// <summary>
    /// 组织类型
    /// </summary>
    public enum OrgnizationType {
        /// <summary>
        /// 默认空
        /// </summary>
        Empty,
        /// <summary>
        /// 公司
        /// </summary>
        COMPANY = 1,
        /// <summary>
        /// 销售域
        /// </summary>
        SALES_CENTER = 2,
        /// <summary>
        /// 采购域
        /// </summary>
        SUPPLY_CENTER = 3,
        /// <summary>
        /// 服务域
        /// </summary>
        SERVICE_CENTER = 4,
        /// <summary>
        /// 工厂
        /// </summary>
        PLANT = 5,
        /// <summary>
        /// 部门
        /// </summary>
        ADMIN_UNIT = 6,
        /// <summary>
        /// 工作中心
        /// </summary>
        WORK_CENTER = 7
    }

    /// <summary>
    /// 比较操作符
    /// </summary>
    /// <remarks>在拥有CompareOperator操作符枚举值的基础上，增加新值，当CompareOperator更新时，需要同步更新</remarks>
    public enum OperatorTypeEnum {
        [Description("当前")]
        Current = 0,

        [Description("=")]
        Equal = 1,

        [Description("<>")]
        NotEqual = 6,

        [Description(">")]
        GreaterThan = 2,

        [Description(">=")]
        /// <summary>
        /// 大于等于 
        /// </summary>
        GreaterEqual = 3,

        [Description("<")]
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 4,

        [Description("<=")]
        /// <summary>
        /// 小于等于
        /// </summary>
        LessEqual = 5,

        [Description("起讫")]
        /// <summary>
        /// 起止
        /// </summary>
        Between = 7,

        [Description("存在于")]
        /// <summary>
        /// 在值列表中
        /// </summary>
        In = 8,

        [Description("包含")]
        /// <summary>
        /// 在值列表中
        /// </summary>
        Like = 9,

        [Description("引用单元格")]
        /// <summary>
        /// 引用单元格
        /// </summary>
        ExcelCell = 10
    }
}
