using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common.Torridity.Metadata;
using Digiwin.Common.Torridity;
using System.ComponentModel;
using Digiwin.ERP.Common.Utils;

namespace Digiwin.ERP.TFL.UI.Implement {
    public class UIEntity {
        public static List<FunctionEntity> CreateViewEntity(DependencyObjectCollection collFunc) {
            var group = collFunc.GroupBy(c => new { FUNCTION_INFO_NAME = c["FUNCTION_INFO_NAME"].ToStringExtension(),
                                                    FUNCTION_INFO_CLASS = c["FUNCTION_INFO_CLASS"].ToStringExtension(),
                                                    FUNCTION_INFO_DESC = c["FUNCTION_INFO_DESC"].ToStringExtension(),
                                                    FUNCTION_INFO_CLASS_NAME = c["FUNCTION_INFO_CLASS_NAME"].ToStringExtension()
            });
            foreach (var item in group) {

                List<FunctionEntity> list = new List<FunctionEntity>();
                FunctionEntity entity = new FunctionEntity() { Name = "TestFunc1", DisplayName = "测试函数1", Description = "测试函数1", Category = "销售" };
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_ID", OffSet = "@a", PropertyName = "DOC_ID", SecondPropertyName = "DOC_ID2", ControlType = ControlTypeEnum.SelectControl, ParaTypeFlag = ParaTypeEnum.String, OrgType = OrgnizationType.Company, ConditionName = "Condition_DOC_ID", DisplayName = "单据类型", QueryProjectId = "QPD_001", QueryTypeKey = "TFL", Tip = "查询单据性质" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.ApproveStatus", PropertyName = "ApproveStatus", SecondPropertyName = "ApproveStatus2", OffSet = "@a", ControlType = ControlTypeEnum.PickList, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_ApproveStatus", PickListTypeName = "BusinessApproveStatus", DisplayName = "状态" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.DOC_DATE", PropertyName = "DOC_DATE", SecondPropertyName = "DOC_DATE2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.DataTime, ConditionName = "Condition_DOC_DATE", DisplayName = "单据日期" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.QTY", PropertyName = "QTY", SecondPropertyName = "QTY2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Decimal, ConditionName = "Condition_QTY", DisplayName = "数量" });
                entity.ParaList.Add(new ParaEntity() { Name = "c.IsMoLot", PropertyName = "IsMoLot", SecondPropertyName = "IsMoLot2", OffSet = "@c", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Boolean, ConditionName = "Condition_IsMoLot", DisplayName = "批工单" });
                entity.ParaList.Add(new ParaEntity() { Name = "c.Year", PropertyName = "Year", SecondPropertyName = "Year2", OffSet = "@c", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_Year", DisplayName = "期间" });
                list.Add(entity);
                entity = new FunctionEntity() { Name = "TestFunc2", DisplayName = "测试函数2", Description = "测试函数1", Category = "销售" };
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_ID", PropertyName = "DOC_ID", SecondPropertyName = "DOC_ID2", OffSet = "@a", ControlType = ControlTypeEnum.SelectControl, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_DOC_ID", DisplayName = "单据类型", QueryProjectId = "QPD_001", QueryTypeKey = "TFL", Tip = "查询单据性质" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.ApproveStatus", PropertyName = "ApproveStatus", SecondPropertyName = "ApproveStatus2", OffSet = "@a", ControlType = ControlTypeEnum.PickList, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_ApproveStatus", PickListTypeName = "BusinessApproveStatus", DisplayName = "状态" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.DOC_DATE", PropertyName = "DOC_DATE", SecondPropertyName = "DOC_DATE2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.DataTime, ConditionName = "Condition_DOC_DATE", DisplayName = "发货日期" });
                list.Add(entity);
                entity = new FunctionEntity() { Name = "TestFunc3", DisplayName = "测试函数3", Description = "测试函数1", Category = "销售" };
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_ID", PropertyName = "DOC_ID", SecondPropertyName = "DOC_ID2", OffSet = "@a", ControlType = ControlTypeEnum.SelectControl, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_DOC_ID", DisplayName = "单据类型", QueryProjectId = "QPD_001", QueryTypeKey = "TFL", Tip = "查询单据性质" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.ApproveStatus", PropertyName = "ApproveStatus", SecondPropertyName = "ApproveStatus2", OffSet = "@a", ControlType = ControlTypeEnum.PickList, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_ApproveStatus", PickListTypeName = "BusinessApproveStatus", DisplayName = "状态" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_DATE", PropertyName = "DOC_DATE", SecondPropertyName = "DOC_DATE2", OffSet = "@a", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Decimal, ConditionName = "Condition_DOC_DATE", DisplayName = "单据日期" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.QTY", PropertyName = "QTY", SecondPropertyName = "QTY2", OffSet = "@a", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Decimal, ConditionName = "Condition_QTY", DisplayName = "数量" });
                list.Add(entity);

                entity = new FunctionEntity() { Name = "TestFunc1", DisplayName = "测试函数1", Description = "测试函数1", Category = "采购" };
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_ID", OffSet = "@a", PropertyName = "DOC_ID", SecondPropertyName = "DOC_ID2", ControlType = ControlTypeEnum.SelectControl, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_DOC_ID", DisplayName = "单据类型", QueryProjectId = "QPD_001", QueryTypeKey = "TFL", Tip = "查询单据性质" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.ApproveStatus", PropertyName = "ApproveStatus", SecondPropertyName = "ApproveStatus2", OffSet = "@a", ControlType = ControlTypeEnum.PickList, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_ApproveStatus", PickListTypeName = "BusinessApproveStatus", DisplayName = "状态" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.DOC_DATE", PropertyName = "DOC_DATE", SecondPropertyName = "DOC_DATE2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.DataTime, ConditionName = "Condition_DOC_DATE", DisplayName = "单据日期" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.QTY", PropertyName = "QTY", SecondPropertyName = "QTY2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Decimal, ConditionName = "Condition_QTY", DisplayName = "数量" });
                entity.ParaList.Add(new ParaEntity() { Name = "c.IsMoLot", PropertyName = "IsMoLot", SecondPropertyName = "IsMoLot2", OffSet = "@c", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.Boolean, ConditionName = "Condition_IsMoLot", DisplayName = "批工单" });
                list.Add(entity);
                entity = new FunctionEntity() { Name = "TestFunc2", DisplayName = "测试函数2", Description = "测试函数1", Category = "采购" };
                entity.ParaList.Add(new ParaEntity() { Name = "a.DOC_ID", PropertyName = "DOC_ID", SecondPropertyName = "DOC_ID2", OffSet = "@a", ControlType = ControlTypeEnum.SelectControl, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_DOC_ID", DisplayName = "单据类型", QueryProjectId = "QPD_001", QueryTypeKey = "TFL", Tip = "查询单据性质" });
                entity.ParaList.Add(new ParaEntity() { Name = "a.ApproveStatus", PropertyName = "ApproveStatus", SecondPropertyName = "ApproveStatus2", OffSet = "@a", ControlType = ControlTypeEnum.PickList, ParaTypeFlag = ParaTypeEnum.String, ConditionName = "Condition_ApproveStatus", PickListTypeName = "BusinessApproveStatus", DisplayName = "状态" });
                entity.ParaList.Add(new ParaEntity() { Name = "b.DOC_DATE", PropertyName = "DOC_DATE", SecondPropertyName = "DOC_DATE2", OffSet = "@b", ControlType = ControlTypeEnum.Input, ParaTypeFlag = ParaTypeEnum.DataTime, ConditionName = "Condition_DOC_DATE", DisplayName = "发货日期" });
                list.Add(entity);
            }

            return list;
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

            formulaResult = @"context.FL(""" + this.Name + @""",""" + formulaResult + @""")";

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
            }

            if (value != string.Empty) {
                value += "]";
                return pre += value;
            }
            return string.Empty;
        }

        private string GetValue(ParaEntity para, string oper) {
            string value = string.Empty;
            bool strFlag = (para.ParaTypeFlag == ParaTypeEnum.String || para.ParaTypeFlag == ParaTypeEnum.DataTime);
            if (oper == "in") {
                foreach (ValueEntity item in para.ParaValueList) {
                    if (Maths.IsEmpty(item.Value))
                        continue;
                    if (strFlag) {
                        if (value == string.Empty)
                            value = "'" + item.Value + "'";
                        else
                            value += "," + "'" + item.Value + "'";
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
                    if (strFlag) {
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
                    if (strFlag) {
                        if (para.Operator == OperatorTypeEnum.ExcelCell) {
                            value = para.Name + oper + @"'""&" + tmpValue + @"&""'";
                        } else {
                            value = para.Name + oper + "'" + tmpValue + "'";
                        }

                    } else {
                        value = para.Name + oper + tmpValue;
                    }
                }
            }

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
        /// 由于Name可能是a.STATUS,鉴于额E10在绑定时对小数点符号的限制，没有办法直接用Name绑定，就用这个属性代替
        /// </summary>
        public string PropertyName {
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
                } else if (this.ParaTypeFlag == ParaTypeEnum.DataTime) {
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
                else if (this.ParaTypeFlag == ParaTypeEnum.DataTime)
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
        PickList = 0,
        /// <summary>
        /// 开窗
        /// </summary>
        SelectControl = 1,
        /// <summary>
        /// 录入框
        /// </summary>
        Input,
        /// <summary>
        /// 日期框
        /// </summary>
        DateControl
    }

    public enum ParaTypeEnum {
        /// <summary>
        /// 日期
        /// </summary>
        DataTime = 2,
        /// <summary>
        /// 数字
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 4,
        /// <summary>
        /// bool类型
        /// </summary>
        Boolean = 5
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
        Company = 1,
        /// <summary>
        /// 销售域
        /// </summary>
        SalesCenter = 2,
        /// <summary>
        /// 采购域
        /// </summary>
        PurchaseCenter = 3,
        /// <summary>
        /// 服务域
        /// </summary>
        ServiceCenter = 4,
        /// <summary>
        /// 工厂
        /// </summary>
        Plant = 5,
        /// <summary>
        /// 部门
        /// </summary>
        Dept = 6,
        /// <summary>
        /// 工作中心
        /// </summary>
        WorkCenter = 7
    }

    /// <summary>
    /// 比较操作符
    /// </summary>
    /// <remarks>在拥有CompareOperator操作符枚举值的基础上，增加新值，当CompareOperator更新时，需要同步更新</remarks>
    public enum OperatorTypeEnum {
        [Description("当前")]
        Current,

        [Description("=")]
        Equal,

        [Description("!=")]
        NotEqual,

        [Description(">")]
        GreaterThan,

        [Description(">=")]
        /// <summary>
        /// 大于等于 
        /// </summary>
        GreaterEqual,

        [Description("<")]
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,

        [Description("<=")]
        /// <summary>
        /// 小于等于
        /// </summary>
        LessEqual,

        [Description("起止")]
        /// <summary>
        /// 起止
        /// </summary>
        Between,

        [Description("存在于")]
        /// <summary>
        /// 在值列表中
        /// </summary>
        In,

        [Description("引用单元格")]
        /// <summary>
        /// 引用单元格
        /// </summary>
        ExcelCell
    }
}
