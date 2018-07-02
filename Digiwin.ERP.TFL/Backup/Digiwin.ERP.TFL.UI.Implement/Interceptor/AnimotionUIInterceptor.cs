//20170829 modi by shenbao for P001-170829002 指标树函数信息支持多语言
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common;
using Digiwin.Common.UI;
using System.Drawing;
using System.Windows.Forms;
using Digiwin.Common.Torridity;
using Digiwin.Common.Torridity.Metadata;
using Digiwin.Common.Services;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Digiwin.ERP.Common.Utils;
using System.ComponentModel;
using Digiwin.ERP.TFL.Business;
using Digiwin.ERP.GetTFLFunctionService.Business;
using System.Collections;
using Digiwin.ERP.TFL.UI.Implement.Properties;

namespace Digiwin.ERP.TFL.UI.Implement {
    [EventInterceptorClass]
    public sealed class AnimotionUIInterceptor : ServiceComponent {
        private List<FunctionEntity> _funtionList = null;
        /*布局控件
         * 列说明：目前布局采用5列模式 没一行对应一个参数
         * 第一列：标签列
         * 第二列：条件列
         * 第三列、第四列：参数列  （因为条件可能是从……到……）
         * 第五列 占位列
         * 行说明：目前布局行数为参数数量+2
         * 第一行：占位行
         * 第二行至倒数第二行为参数布局行
         * 最后一行：占位行
         */
        private TableLayoutPanel _table;
        private TabControl _tabControl;
        private EditorView _editView;
        private const int ColumnCount = 5;
        private const int RowHeight = 25;
        /// <summary>
        /// 标签列的宽度
        /// 如果是非英文：100
        /// 英文：取参数最大宽度
        /// </summary>
        private int TitleColumnWidth = 100;  //20170829 modi by shenbao for P001-170829002

        //虚拟绑定实体数据源
        private DependencyObject _uiViewDataSource = null;

        //当前editview的真实数据源
        private DependencyObject _dataSource = null;
        
        /// <summary>
        ///  界面行数
        ///  多2行用于界面顶端和低端的占位
        /// </summary>
        public int RowCount
        {
            get;
            set;
        }

        public string CurrentFunctionName
        {
            get;
            set;
        }

        public string CurrentFuncCatetory {
            get;
            set;
        }

        private IUserParameterService _paraSrv;
        private IUserParameterService ParaSrv {
            get {
                if (_paraSrv == null)
                    _paraSrv = this.GetService<IUserParameterService>();
                return _paraSrv;
            }
        }

        Dictionary<string, ValueEntity> sysParameters = new Dictionary<string, ValueEntity>();
        private string _company = string.Empty;
        /// <summary>
        /// 当前公司编码
        /// </summary>
        private string Company {
            get {
                if (_company == string.Empty) {
                    DependencyObject obj = this.GetServiceForThisTypeKey<IQueryDataService>().QueryCompany(ParaSrv.GetValue("DEF_COMPANY_ID").ToStringExtension());
                    if (obj != null)
                        _company = obj["COMPANY_CODE"].ToStringExtension();
                }

                return _company;
            }
        }

        public FunctionEntity CurrentFunction
        {
            get {
                return _funtionList.FirstOrDefault(c => c.Name == CurrentFunctionName);
            }
        }

        [EventInterceptor(typeof(IEditorView), "Load")]
        private void CreateUIControls(object sender, EventArgs e) {
            #region //20170829 mark by shenbao for P001-170829002 由于excel报表点击左侧grid是新建一个editorview，load里面无法拿到当前的函数，转移到DataSourceChanged中处理
            //_editView = sender as EditorView;
            //IFindControlService findControlSrv = this.GetService<IFindControlService>();
            //Control control = null;
            //DependencyObjectCollection coll = this.GetServiceForThisTypeKey<IQueryDataService>().GetFunctionInfo();
            //_funtionList = UIEntity.CreateViewEntity(coll,this as ServiceComponent);
            //if (findControlSrv.TryGet("desingerTabControl1", out control)) {
            //    _tabControl = control as TabControl;
            //    _tabControl.SuspendLayout();
            //    _tabControl.TabPages[0].SuspendLayout();
            //    CreateTableLayout(sender as EditorView);
            //    _tabControl.TabPages[0].Controls.Add(_table);
            //    _tabControl.TabPages[0].ResumeLayout(false);
            //    _tabControl.ResumeLayout(false);
            //    _tabControl.PerformLayout();
            //}
            #endregion
        }

        [EventInterceptor(typeof(IEditorView), "DataSourceChanged")]
        private void DataSourceChange(object sender, EventArgs e)
        {
            ICurrentDocumentWindow win = this.GetServiceForThisTypeKey<ICurrentDocumentWindow>();
            if (win != null)
            {
                _dataSource = win.EditController.EditorView.DataSource as DependencyObject;
                InitSysParameter();
                CurrentFunctionName = _dataSource["CURRENT_FUNCTION_NAME"].ToStringExtension();
                CurrentFuncCatetory = _dataSource["CURRENT_CATEGORY_CODE"].ToStringExtension();

                //20170829 add by shenbao for P001-170829002 ===begin===
                _editView = sender as EditorView;
                IFindControlService findControlSrv = this.GetService<IFindControlService>();
                Control control = null;
                DependencyObjectCollection coll = this.GetServiceForThisTypeKey<IQueryDataService>().GetFunctionInfo();
                _funtionList = UIEntity.CreateViewEntity(coll, this as ServiceComponent);
                if (findControlSrv.TryGet("desingerTabControl1", out control)) {
                    _tabControl = control as TabControl;
                    _tabControl.SuspendLayout();
                    _tabControl.TabPages[0].SuspendLayout();
                    CreateTableLayout(sender as EditorView);
                    _tabControl.TabPages[0].Controls.Add(_table);
                    _tabControl.TabPages[0].ResumeLayout(false);
                    _tabControl.ResumeLayout(false);
                    _tabControl.PerformLayout();
                }
                //20170829 add by shenbao for P001-170829002 ===end===

                if (CurrentFunction != null) {
                    CreateViewDataSource();
                    CreateUIView();
                    InitViewData();
                    CheckControl();
                }
            }
        }

        void btn_Click(object sender, ButtonPressedEventArgs e) {
            ButtonEdit btn = sender as ButtonEdit;
            DependencyObjectCollection coll = btn.Tag as DependencyObjectCollection;
            if (coll == null)
                return;

            ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName == btn.Name);
            if (para != null) {
                using (DocFWOForm form = new DocFWOForm(new Guid(), "", coll, this.ResourceServiceProvider)) {
                    SelectWindowHelper.AdjustWindowLocation(sender as Control, form);
                    form.Text = para.Tip;
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK) {
                        _uiViewDataSource[para.PropertyName] = form.RtnValues;
                    }
                }
            }
        }

        private void InitSysParameter() {
            if (_dataSource != null) {
                DependencyObject obj = this.GetServiceForThisTypeKey<IQueryDataService>().QueryCompany(_dataSource["InitCompanyId"].ToStringExtension());
                ValueEntity value = new ValueEntity();
                value.Value = _dataSource["InitCompanyId"];
                value.DisplayText = obj["COMPANY_NAME"].ToStringExtension();
                value.HiddenText = obj["COMPANY_CODE"].ToStringExtension();
                sysParameters.Add("COMPANY", value);
                _dataSource["FORMULA_RESULT"] = string.Empty;
            }
        }

        /// <summary>
        /// 创建布局控件
        /// </summary>
        private void CreateTableLayout(EditorView ev)
        {
            RowCount = Math.Floor(this._editView.Height.ToDouble() / RowHeight).ToInt32();

            _table = new TableLayoutPanel();
            _table.Dock = DockStyle.Fill;
            _table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            _table.ColumnCount = ColumnCount;
            //_table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));   //20170829 mark by shenbao for P001-170829002
            //20170829 add by shenbao for P001-170829002 ===begin===
            _table.AutoScroll = true;
            ILogOnService logSrv = this.GetService<ILogOnService>();
            if (logSrv.LogOnLanguage != "ENU") {  //非英文时100已经够用
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            } else {
                TitleColumnWidth = GetFunctionMaxLength(ev.FindForm());
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, TitleColumnWidth));
            }
            //20170829 add by shenbao for P001-170829002 ===end===
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));  //20170829 modi by shenbao for P001-170829002  100==>10 占位列10就够了
            _table.RowCount = this.RowCount;
            for (int i = 0; i < this.RowCount; i++) {
                if (i == 0) {
                    _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));  //第一行作为站位行
                } else if (i == this.RowCount - 1) {   //20170829 add by shenbao for P001-170829002 最后一行也是站位行
                    _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
                } else {
                    _table.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));
                }
            }
        }

        //20170829 add by shenbao for P001-170829002
        /// <summary>
        /// 获取显示名称最长的一项
        /// 不够100默认100
        /// </summary>
        private int GetFunctionMaxLength(Form form) {
            if (_funtionList == null || _funtionList.Count <= 0
                || CurrentFunction == null)
                return 100;
            string maxLengthName = string.Empty;
            foreach (var item in CurrentFunction.ParaList) {
                if (item.DisplayName.Length > maxLengthName.Length) {
                    maxLengthName = item.DisplayName;
                }
            }

            int maxLength = 100;
            //以界面的默认字体渲染
            using (Graphics g = form.CreateGraphics()) {
                SizeF size = g.MeasureString(maxLengthName, form.Font);
                int width = Convert.ToInt32(size.Width);  //必须用Convert，这里是小数，用ToInt32会有问题
                maxLength = (width > maxLength ? width : maxLength);
            }

            return (maxLength + 4);  //添加4个宽带的边框
        }

        void lookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            CurrentFunctionName = (sender as LookUpEdit).EditValue.ToStringExtension();
            CreateViewDataSource();
            CreateUIView();
            InitViewData();
        }

        /// <summary>
        /// 切换函数时，初始化以前设置的参数
        /// 暂时不用
        /// </summary>
        private void InitViewData2() {
            //先取消订阅事件，否则会导致ParaList被界面重绘
            _uiViewDataSource.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);
            try {
                foreach (ParaEntity item in CurrentFunction.ParaList) {
                    if (item.ParaValueList.Count > 0) {
                        bool flag = false;  //存在于控件标记
                        if (item.Operator == OperatorTypeEnum.In) {
                            flag = true;
                            if (item.ControlType == ControlTypeEnum.SelectControl) {
                                _uiViewDataSource[item.PropertyName] = Maths.GuidDefaultValue();
                                _uiViewDataSource[item.PropertyName.Replace("_ID", "_CODE")] = string.Join(";", item.ParaValueList.Select(c => c.HiddenText.ToStringExtension()).ToArray());
                                _uiViewDataSource[item.PropertyName.Replace("_ID", "_NAME")] = string.Join(";", item.ParaValueList.Select(c => c.DisplayText.ToStringExtension()).ToArray());

                            } else {
                                _uiViewDataSource[item.PropertyName] = string.Join(";", item.ParaValueList.Select(c => c.Value.ToStringExtension()).ToArray());
                            }
                        } else if (item.ControlType == ControlTypeEnum.SelectControl) {
                            _uiViewDataSource[item.PropertyName.Replace("_ID", "_CODE")] = item.ParaValueList[0].HiddenText;
                            _uiViewDataSource[item.PropertyName.Replace("_ID", "_NAME")] = item.ParaValueList[0].DisplayText;
                        }

                        if (!flag) {
                            _uiViewDataSource[item.PropertyName] = item.ParaValueList[0].Value;
                        }

                        if (item.Operator == OperatorTypeEnum.Between) {
                            _uiViewDataSource[item.SecondPropertyName] = item.ParaValueList[1].Value;
                            if (item.ControlType == ControlTypeEnum.SelectControl) {
                                _uiViewDataSource[item.SecondPropertyName.Replace("_ID", "_CODE")] = item.ParaValueList[1].HiddenText;
                                _uiViewDataSource[item.SecondPropertyName.Replace("_ID", "_NAME")] = item.ParaValueList[1].DisplayText;
                            }
                        }
                    }
                }
            } finally {
                _uiViewDataSource.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);
            }
            _dataSource["FORMULA_RESULT"] = CurrentFunction.CalculteFormulaResult();
        }

        private void InitViewData() {
            foreach (ParaEntity item in CurrentFunction.ParaList) {
                if (item.ParaValueList.Count > 0) {
                    if (item.ControlType == ControlTypeEnum.SelectControl && item.OrgType == OrgnizationType.COMPANY) {
                        if (sysParameters.ContainsKey("COMPANY")) {
                            _uiViewDataSource[item.PropertyName] = sysParameters["COMPANY"].Value;
                            _uiViewDataSource[item.PropertyName.Replace("_ID", "_CODE")] = sysParameters["COMPANY"].HiddenText;
                            _uiViewDataSource[item.PropertyName.Replace("_ID", "_NAME")] = sysParameters["COMPANY"].DisplayText;
                            _uiViewDataSource[item.PropertyName + "_RETURN_VALUE"] = sysParameters["COMPANY"].HiddenText;
                        }
                    }
                }
            }
            _dataSource["FORMULA_RESULT"] = CurrentFunction.CalculteFormulaResult();
        }

        /// <summary>
        /// 创建界面绑定view
        /// </summary>
        /// <returns></returns>
        private void CreateViewDataSource()
        {
            DependencyObjectType type = new DependencyObjectType("Type");
            foreach (ParaEntity para in CurrentFunction.ParaList)
            {
                type.RegisterSimpleProperty(para.ConditionName, typeof(string));
                type.RegisterSimpleProperty(para.PropertyName, para.ParaType);
                type.RegisterSimpleProperty(para.SecondPropertyName, para.ParaType);  //该字段用于操作类型选择between时起作用
                if (para.ControlType == ControlTypeEnum.SelectControl)
                {
                    type.RegisterSimpleProperty(para.PropertyName.Replace("_ID", "_NAME"), typeof(string));
                    type.RegisterSimpleProperty(para.PropertyName.Replace("_ID", "_CODE"), typeof(string));
                    //注册一个固定的返回值控件
                    type.RegisterSimpleProperty(para.PropertyName + "_RETURN_VALUE", typeof(string));

                    type.RegisterSimpleProperty(para.SecondPropertyName.Replace("_ID", "_NAME"), typeof(string));
                    type.RegisterSimpleProperty(para.SecondPropertyName.Replace("_ID", "_CODE"), typeof(string));
                    //注册一个固定的返回值控件
                    type.RegisterSimpleProperty(para.SecondPropertyName + "_RETURN_VALUE", typeof(string));
                }
            }

            DependencyObject obj = new DependencyObject(type);

            _uiViewDataSource = obj;
            _uiViewDataSource.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);
        }

        /// <summary>
        /// 组建界面
        /// </summary>
        /// <param name="dataSource"></param>
        private void CreateUIView() {
            if (CurrentFunction == null)
                return;

            int index = 1;
            int row = 1;
            _editView.SuspendLayout();
            _tabControl.SuspendLayout();
            _tabControl.TabPages[0].SuspendLayout();
            _table.SuspendLayout();
            ClearControls();
            foreach (ParaEntity para in CurrentFunction.ParaList) {
                //标签
                Label lbl = ControlUtils.CreateLabel("lbl", para.DisplayName, false, TitleColumnWidth);
                _table.Controls.Add(lbl, 0, row);

                //条件选项
                string defaultValueCondition = ((int)para.Operator).ToStringExtension();
                if (para.OrgType == OrgnizationType.COMPANY) {
                    defaultValueCondition = ((int)OperatorTypeEnum.Current).ToStringExtension();
                }
                LookUpEdit condition = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.ConditionName, para.ConditionName, false, true, 0, "-1", 2);
                BindConditionData(condition, para);
                _table.Controls.Add(condition, 1, row);

                //参数
                #region 控件类型
                //以枚举ControlTypeEnum优先判断
                if (para.ControlType == ControlTypeEnum.SelectControl) {
                    DigiwinSelectControl select = ControlUtils.CreateSelectControl(_uiViewDataSource, para.PropertyName, para.PropertyName, para, para.OrgType == OrgnizationType.COMPANY, true, false, index++);
                    select.Anchor = AnchorStyles.Left;
                    select.Width = 150;
                    _table.Controls.Add(select, 2, row);

                    select = ControlUtils.CreateSelectControl(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, para, false, para.Operator == OperatorTypeEnum.Between, false, index++);
                    select.Anchor = AnchorStyles.Left;
                    select.Width = 150;
                    _table.Controls.Add(select, 3, row);
                } else if (para.ControlType == ControlTypeEnum.PickList) {
                    LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.PropertyName, para.PropertyName, true, true, 0, string.Empty, index++);
                    lookup.Anchor = AnchorStyles.Left;
                    lookup.Width = 150;
                    BindCustomPickList(para.PickListTypeName, lookup);
                    _table.Controls.Add(lookup, 2, row);

                    lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, true, para.Operator == OperatorTypeEnum.Between, 0, null, index++);
                    lookup.Anchor = AnchorStyles.Left;
                    lookup.Width = 150;
                    BindCustomPickList(para.PickListTypeName, lookup);
                    _table.Controls.Add(lookup, 3, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.DateTime || para.ParaTypeFlag == ParaTypeEnum.Date) {
                    DigiwinDateTimePicker date = ControlUtils.CreateDateTimePicker(_uiViewDataSource, para.PropertyName, para.PropertyName, false, true, para.ParaTypeFlag == ParaTypeEnum.DateTime, index++);
                    _table.Controls.Add(date, 2, row);

                    date = ControlUtils.CreateDateTimePicker(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, false, para.Operator == OperatorTypeEnum.Between, para.ParaTypeFlag == ParaTypeEnum.DateTime, index++);
                    _table.Controls.Add(date, 3, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.Decimal) {
                    DigiwinTextBox dec = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.PropertyName, para.PropertyName, "0", true, false, true, 10, index++);
                    dec.Width = 150;
                    _table.Controls.Add(dec, 2, row);

                    dec = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, "0", true, false, para.Operator == OperatorTypeEnum.Between, 10, index++);
                    dec.Width = 150;
                    _table.Controls.Add(dec, 3, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.String) {
                    DigiwinTextBox str = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.PropertyName, para.PropertyName, "0", false, false, true, 10, index++);
                    str.Width = 150;
                    _table.Controls.Add(str, 2, row);

                    str = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, "0", false, false, para.Operator == OperatorTypeEnum.Between, 10, index++);
                    str.Width = 150;
                    _table.Controls.Add(str, 3, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.Boolean) {  //bool类型也采用picklist模式
                    LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.PropertyName, para.PropertyName, true, true, 0, string.Empty, index++);
                    lookup.Anchor = AnchorStyles.Left;
                    lookup.Width = 150;
                    BindCustomBool(lookup);
                    _table.Controls.Add(lookup, 2, row);

                    lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, true, para.Operator == OperatorTypeEnum.Between, 0, string.Empty, index++);
                    lookup.Anchor = AnchorStyles.Left;
                    lookup.Width = 150;
                    BindCustomBool(lookup);
                    _table.Controls.Add(lookup, 3, row);
                } else {
                    DigiwinTextBox str = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.PropertyName, para.PropertyName, "0", false, false, true, 10, index++);
                    str.Width = 150;
                    _table.Controls.Add(str, 2, row);

                    str = ControlUtils.CreateNumericTextBox(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, "0", false, false, para.Operator == OperatorTypeEnum.Between, 10, index++);
                    str.Width = 150;
                    _table.Controls.Add(str, 3, row);
                }
                #endregion
                para.Row = row;
                para.Column = 2;
                _uiViewDataSource[para.ConditionName] = ((int)para.Operator).ToStringExtension();

                row++;
            }
            _table.ResumeLayout(false);
            _table.PerformLayout();
            _tabControl.TabPages[0].ResumeLayout(false);
            _tabControl.TabPages[0].PerformLayout();
            _tabControl.ResumeLayout(false);
            _tabControl.PerformLayout();
            _editView.ResumeLayout(false);
            _editView.PerformLayout();
        }

        private void BindConditionData(LookUpEdit lookUpEdit, ParaEntity para) {
            DependencyObjectType type = new DependencyObjectType("Condition");
            type.RegisterSimpleProperty("Value", typeof(string));
            type.RegisterSimpleProperty("DisplayName", typeof(string));

            DependencyObjectCollection coll = new DependencyObjectCollection(type);
            if (Maths.IsNotEmpty(para.Conditions)) {
                string[] conditions = para.Conditions.Split(';');
                List<string> compareConditions = new List<string> { "Current", "=", "!=", ">", ">=", "<", "<=", "Between", "In", "Like", "ExcelCell" };

                foreach (string str in conditions) {
                    if (!compareConditions.Contains(str))
                        continue;
                    //对于decimal、日期、string类型（录入框类的，因为picklist也又可以是string类型的，但是它可以使用存在于）
                    //这些类型in没有意义
                    if (str == "In" && (para.ParaTypeFlag == ParaTypeEnum.Decimal
                        || para.ParaTypeFlag == ParaTypeEnum.DateTime
                        || para.ParaTypeFlag == ParaTypeEnum.Date
                        || (para.ParaTypeFlag == ParaTypeEnum.String && para.ControlType == ControlTypeEnum.Input)))
                        continue;

                    //bool类型只有=,!=,in条件
                    if (para.ParaTypeFlag == ParaTypeEnum.Boolean && str != "=" && str != "!=" && str != "In")
                        continue;

                    DependencyObject obj = new DependencyObject(type);
                    obj["Value"] = GetConditionCompareEnumValue(str).ToStringExtension();
                    obj["DisplayName"] = GetConditionDisplayName(str);
                    coll.Add(obj);
                }
            }

            ControlUtils.BindSimpleCustomDataSource(coll, lookUpEdit);
        }

        private int GetConditionCompareEnumValue(string para) {
            switch (para) {
                case "Current":
                    return (int)OperatorTypeEnum.Current;
                case "=":
                    return (int)OperatorTypeEnum.Equal;
                case "!=":
                    return (int)OperatorTypeEnum.NotEqual;
                case ">":
                    return (int)OperatorTypeEnum.GreaterThan;
                case ">=":
                    return (int)OperatorTypeEnum.GreaterEqual;
                case "<":
                    return (int)OperatorTypeEnum.LessThan;
                case "<=":
                    return (int)OperatorTypeEnum.LessEqual;
                case "Between":
                    return (int)OperatorTypeEnum.Between;
                case "In":
                    return (int)OperatorTypeEnum.In;
                case "Like":
                    return (int)OperatorTypeEnum.Like;
                case "ExcelCell":
                    return (int)OperatorTypeEnum.ExcelCell;
            }

            return (int)OperatorTypeEnum.Equal;
        }

        private string GetConditionDisplayName(string para) {
            switch (para) {
                case "=":
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":
                    return para;
                case "Between":
                    return Resources.LABEL_001007;
                case "In":
                    return Resources.LABEL_001008;
                case "Like":
                    return Resources.LABEL_001009;
                case "Current":
                    return Resources.LABEL_001010;
                case "ExcelCell":
                    return Resources.LABEL_001011;
            }

            return "=";
        }

        private void ClearControls()
        {
            while (_table.Controls.Count > 0)//保留第一行的2个控件
                _table.Controls.RemoveAt(0);
        }

        /// <summary>
        /// 创建picklist的datasource
        /// 需要添加空白行
        /// </summary>
        /// <param name="pickListName"></param>
        /// <param name="lookUpEdit"></param>
        /// <returns></returns>
        private DependencyObjectCollection BindCustomPickList(string pickListName, LookUpEdit lookUpEdit)
        {
            DependencyObjectCollection items = GetPickListDataource(pickListName, false);

            ControlUtils.BindSimpleCustomDataSource(items, lookUpEdit);

            return items;
        }

        private DependencyObjectCollection BindCustomBool(LookUpEdit lookUpEdit) {
            DependencyObjectCollection items = GetCustomBoolDataSource(lookUpEdit.Name, false);

            ControlUtils.BindSimpleCustomDataSource(items, lookUpEdit);

            return items;
        }

        private DependencyObjectCollection GetCustomBoolDataSource(string name, bool allowCheck) {
            DependencyObjectType type = new DependencyObjectType(name);
            type.RegisterSimpleProperty("Value", typeof(string));
            type.RegisterSimpleProperty("DisplayName", typeof(string));
            if (allowCheck) {
                type.RegisterSimpleProperty("Select", typeof(bool));
            }
            DependencyObjectCollection items = new DependencyObjectCollection(type);

            DependencyObject obj = null;
            if (!allowCheck) {
                obj = items.AddNew();
                obj["Value"] = "";
                obj["DisplayName"] = "";
                if (allowCheck) {
                    obj["Select"] = false;
                }
            }

            obj = items.AddNew();
            obj["Value"] = "1";
            obj["DisplayName"] = Resources.LABEL_001001;
            if (allowCheck) {
                obj["Select"] = false;
            }

            obj = items.AddNew();
            obj["Value"] = "0";
            obj["DisplayName"] = Resources.LABEL_001002;
            if (allowCheck) {
                obj["Select"] = false;
            }

            return items;
        }

        private DependencyObjectCollection GetPickListDataource(string pickListName,bool allowCheck) {
            if (string.IsNullOrEmpty(pickListName))
                return null;

            IPickListDataService plDataSrv = this.GetService<IPickListDataService>();
            DependencyObjectType type = new DependencyObjectType(pickListName);
            type.RegisterSimpleProperty("Value", typeof(string));
            type.RegisterSimpleProperty("DisplayName", typeof(string));
            if (allowCheck) {
                type.RegisterSimpleProperty("Select", typeof(bool));
            }
            DependencyObjectCollection items = new DependencyObjectCollection(type);

            if (!allowCheck) {
                DependencyObject first = items.AddNew();
                first["Value"] = "";
                first["DisplayName"] = "";
            }
            foreach (var item in plDataSrv.GetPickListSortedData(pickListName)) {
                DependencyObject obj = items.AddNew();
                obj["Value"] = item.Id.ToStringExtension();
                obj["DisplayName"] = item.DisplayName;
                if (allowCheck) {
                    obj["Select"] = false;
                }
            }

            return items;
        }

        void dataSource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            DependencyObject obj = sender as DependencyObject;
            if (obj == null)
                return;
            if (e.PropertyName.StartsWith("Condition_"))
                ChangeControl(obj, e);
            else {
                ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName == e.PropertyName || c.SecondPropertyName == e.PropertyName);
                if (para == null) {   //可能code和name变化
                    para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName.Replace("_ID", "_CODE") == e.PropertyName || c.SecondPropertyName.Replace("_ID", "_CODE") == e.PropertyName);
                    if (para == null) {
                        para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName.Replace("_ID", "_NAME") == e.PropertyName || c.SecondPropertyName.Replace("_ID", "_NAME") == e.PropertyName);
                    }
                }
                if (para != null) {
                    OperatorTypeEnum oper = (OperatorTypeEnum)_uiViewDataSource[para.ConditionName].ToInt32();
                    if (oper == OperatorTypeEnum.In) {
                        if (para.ControlType != ControlTypeEnum.SelectControl) {
                            para.ParaValueList.Clear();
                            string[] vas = _uiViewDataSource[para.PropertyName].ToStringExtension().Split(';');
                            vas = _uiViewDataSource[para.PropertyName].ToStringExtension().Split(';');
                            if (vas != null && vas.Length > 0) {
                                foreach (string item in vas) {
                                    ValueEntity value = new ValueEntity() { Value = item, DisplayText = item, HiddenText = item };
                                    para.ParaValueList.Add(value);
                                }
                            }
                        }
                    } else if (oper == OperatorTypeEnum.Between) {
                        if (e.PropertyName == para.PropertyName) {
                            para.ParaValueList[0].Value = _uiViewDataSource[e.PropertyName];
                        } else if (e.PropertyName == para.SecondPropertyName) {
                            para.ParaValueList[1].Value = _uiViewDataSource[e.PropertyName];
                        } else if (e.PropertyName.Replace("_NAME", "_ID") == para.PropertyName) {
                            para.ParaValueList[0].DisplayText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_NAME")].ToStringExtension();
                        } else if (e.PropertyName.Replace("_CODE", "_ID") == para.PropertyName) {
                            para.ParaValueList[0].HiddenText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_CODE")].ToStringExtension();
                        } else if (e.PropertyName.Replace("_NAME", "_ID") == para.SecondPropertyName) {
                            para.ParaValueList[1].DisplayText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_NAME")].ToStringExtension();
                        } else if (e.PropertyName.Replace("_CODE", "_ID") == para.SecondPropertyName) {
                            para.ParaValueList[1].HiddenText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_CODE")].ToStringExtension();
                        }
                    } else {
                        if (para.PropertyName == e.PropertyName) {
                            para.ParaValueList[0].Value = _uiViewDataSource[e.PropertyName];
                            
                        } else if (para.ControlType == ControlTypeEnum.SelectControl) {
                            if (e.PropertyName.EndsWith("_NAME")) {
                                para.ParaValueList[0].DisplayText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_NAME")].ToStringExtension();
                            } else if (e.PropertyName.EndsWith("_CODE")) {
                                para.ParaValueList[0].HiddenText = _uiViewDataSource[e.PropertyName.Replace("_ID", "_CODE")].ToStringExtension();
                            }
                        }
                    }
                } else {  //单独处理开窗返回值的RETURN_VALUE
                    para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName == e.PropertyName.Replace("_RETURN_VALUE",""));
                    if (para != null && para.ControlType == ControlTypeEnum.SelectControl) {
                        para.ParaValueList[0].ReturnValue = _uiViewDataSource[e.PropertyName];
                    }
                    if (para == null) {
                        para = CurrentFunction.ParaList.FirstOrDefault(c => c.SecondPropertyName == e.PropertyName.Replace("_RETURN_VALUE", ""));
                        if (para != null && para.ControlType == ControlTypeEnum.SelectControl) {
                            para.ParaValueList[1].ReturnValue = _uiViewDataSource[e.PropertyName];
                        }
                    }
                }
            }

            _dataSource["FORMULA_RESULT"] = CurrentFunction.CalculteFormulaResult();
        }

        /// <summary>
        /// 切换控件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void ChangeControl(DependencyObject obj,PropertyChangedEventArgs e) {
            ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c => c.ConditionName == e.PropertyName);
            if (para == null)
                return;
            //首先设置操作符
            OperatorTypeEnum oldOper = para.Operator;  //原来的值
            para.Operator = (OperatorTypeEnum)obj[e.PropertyName].ToInt32();
            //清空原来的参数值
            ClearParaValue(para);  
            //如果是between，则显示参数的第二个控件
            Control[] controlSecod = _table.Controls.Find(para.SecondPropertyName, false);
            //调整相关属性
            SetControlProperty(para);
            if (para.Operator == OperatorTypeEnum.Between) {
                //查找该参数的第二个控件
                if (controlSecod != null && controlSecod.Length > 0) {
                    controlSecod[0].Visible = true;
                }
            } else {
                if (controlSecod != null && controlSecod.Length > 0)
                    controlSecod[0].Visible = false;
            }

            #region 切换控件
            if (oldOper == OperatorTypeEnum.In || para.Operator == OperatorTypeEnum.In) {  //只有从x=>存在于，或者存在于=>x时才需要切换
                if (para.ControlType == ControlTypeEnum.PickList) {
                    if (para.Operator == OperatorTypeEnum.In) {
                        _table.SuspendLayout();
                        Control control = _table.GetControlFromPosition(2, para.Row);

                        ButtonEdit btn = ControlUtils.CreateButtonEdit(_uiViewDataSource, para.PropertyName, para.PropertyName, false, true, string.Empty, control.TabIndex);
                        btn.ButtonClick += new ButtonPressedEventHandler(btn_Click);
                        btn.Width = control.Width;
                        btn.Tag = GetPickListDataource(para.PickListTypeName, true);
                        _table.Controls.Remove(control);
                        _table.Controls.Add(btn, 2, para.Row);
                        _table.ResumeLayout(true);
                    } else {
                        _table.SuspendLayout();
                        Control control = _table.GetControlFromPosition(2, para.Row);

                        LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.PropertyName, para.PropertyName, true, true, 0, null, control.TabIndex);
                        lookup.Anchor = AnchorStyles.Left;
                        lookup.Width = 150;
                        lookup.Name = para.PropertyName;
                        BindCustomPickList(para.PickListTypeName, lookup);

                        _table.Controls.Remove(control);
                        _table.Controls.Add(lookup, 2, para.Row);
                        _table.ResumeLayout(true);
                    }

                    CheckControl(para);
                } else if (para.ControlType == ControlTypeEnum.SelectControl) {
                    Control control = _table.GetControlFromPosition(2, para.Row);
                    DigiwinSelectControl select = control as DigiwinSelectControl;

                    if (select != null && select.WindowOpeners.Count > 0) {
                        if (para.Operator == OperatorTypeEnum.In) {
                            GeneralWindowOpener open = select.WindowOpeners[0] as GeneralWindowOpener;
                            if (open != null) {
                                open.MultiSelect = true;
                                select.SelectStyle = SelectStyle.TwoInOneOpen;
                                select.WindowOpeners[0].ExecuteReturnExpression += new EventHandler<ExecuteReturnExpressionEventArgs>(AnimotionUIInterceptor_ExecuteReturnExpression);  //干预脚步，不再执行
                            }
                        } else {
                            GeneralWindowOpener open = select.WindowOpeners[0] as GeneralWindowOpener;
                            if (open != null) {
                                select.WindowOpeners[0].ExecuteReturnExpression -= new EventHandler<ExecuteReturnExpressionEventArgs>(AnimotionUIInterceptor_ExecuteReturnExpression); ;
                                open.MultiSelect = false;
                                select.SelectStyle = SelectStyle.TwoInOneOpen;
                            }
                        }
                    }
                } else if (para.ParaTypeFlag == ParaTypeEnum.Boolean) {
                    if (para.Operator == OperatorTypeEnum.In) {
                        _table.SuspendLayout();
                        Control control = _table.GetControlFromPosition(2, para.Row);

                        ButtonEdit btn = ControlUtils.CreateButtonEdit(_uiViewDataSource, para.PropertyName, para.PropertyName, false, true, string.Empty, control.TabIndex);
                        btn.ButtonClick += new ButtonPressedEventHandler(btn_Click);
                        btn.Width = control.Width;
                        btn.Tag = GetCustomBoolDataSource(para.PropertyName, true);
                        btn.Name = para.PropertyName;
                        _table.Controls.Remove(control);
                        _table.Controls.Add(btn, 2, para.Row);
                        _table.ResumeLayout(true);
                    } else {
                        _table.SuspendLayout();
                        Control control = _table.GetControlFromPosition(2, para.Row);

                        LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_uiViewDataSource, para.SecondPropertyName, para.SecondPropertyName, true, true, 0, string.Empty, control.TabIndex);
                        lookup.Anchor = AnchorStyles.Left;
                        lookup.Width = 150;
                        lookup.Name = para.PropertyName;
                        BindCustomBool(lookup);

                        _table.Controls.Remove(control);
                        _table.Controls.Add(lookup, 2, para.Row);
                        _table.ResumeLayout(true);
                    }
                }
            }
            #endregion
            
        }

        /// <summary>
        /// 切换控件时，修改相关属性，比如只读等
        /// </summary>
        private void SetControlProperty(ParaEntity para) {
            Control control = _table.GetControlFromPosition(2, para.Row);
            if (control != null) {  
                DigiwinSelectControl select = control as DigiwinSelectControl;
                if (select != null) {  //组织为公司，条件为当前时，需要设置只读
                    if (para.Operator != OperatorTypeEnum.Current)
                        select.ReadOnly = false;
                    else {
                        select.ReadOnly = true;
                    }
                }
            }
        }

        private void ClearParaValue(ParaEntity para) {
            if (para.ControlType == ControlTypeEnum.SelectControl) {
                if (para.Operator == OperatorTypeEnum.Current) {
                    if (sysParameters.ContainsKey("COMPANY")) {
                        _uiViewDataSource[para.PropertyName] = sysParameters["COMPANY"].Value;
                        _uiViewDataSource[para.PropertyName.Replace("_ID", "_NAME")] = sysParameters["COMPANY"].DisplayText;
                        _uiViewDataSource[para.PropertyName.Replace("_ID", "_CODE")] = sysParameters["COMPANY"].HiddenText;
                        _uiViewDataSource[para.PropertyName + "_RETURN_VALUE"] = sysParameters["COMPANY"].HiddenText;
                    }
                } else {
                    _uiViewDataSource[para.PropertyName] = Maths.GuidDefaultValue();
                    _uiViewDataSource[para.PropertyName.Replace("_ID", "_CODE")] = string.Empty;
                    _uiViewDataSource[para.PropertyName.Replace("_ID", "_NAME")] = string.Empty;
                    _uiViewDataSource[para.PropertyName + "_RETURN_VALUE"] = string.Empty;
                }
            } else if (para.ControlType == ControlTypeEnum.PickList) {
                _uiViewDataSource[para.PropertyName] = string.Empty;
            } else if (para.ParaTypeFlag == ParaTypeEnum.Boolean) {
                _uiViewDataSource[para.PropertyName] = string.Empty;
            } else if (para.ParaTypeFlag == ParaTypeEnum.DateTime || para.ParaTypeFlag == ParaTypeEnum.Date) {
                _uiViewDataSource[para.PropertyName] = OrmDataOption.EmptyDateTime;
            } else if (para.ParaTypeFlag == ParaTypeEnum.Decimal) {
                _uiViewDataSource[para.PropertyName] = 0;
            } else {
                _uiViewDataSource[para.PropertyName] = string.Empty;
            }
        }

        #region Error
        private void CheckControl(ParaEntity para) {
            if (para.ControlType == ControlTypeEnum.SelectControl) {
                if (Maths.IsEmpty(para.QueryProjectId) || Maths.IsEmpty(para.QueryTypeKey)) {
                    Control control = _table.GetControlFromPosition(para.Column, para.Row);
                    string message = string.Empty;
                    if (control != null) {
                        if (Maths.IsEmpty(para.QueryProjectId)) {
                            message = Resources.LABEL_001003;
                        } else {
                            if (message != string.Empty) {
                                message += "\r\n" + Resources.LABEL_001006;
                            } else {
                                message = Resources.LABEL_001006;
                            }
                        }

                        SetControlError(control, message);
                    }
                }
            } else if (para.ControlType == ControlTypeEnum.PickList && Maths.IsEmpty(para.PickListTypeName)) {
                Control control = _table.GetControlFromPosition(para.Column, para.Row);
                if (control != null) {
                    SetControlError(control, Resources.LABEL_001004);
                }
            }
        }

        private void CheckControl() {
            if (CurrentFunction == null)
                return;
            foreach (var item in CurrentFunction.ParaList) {
                CheckControl(item);
            }
        }

        private void SetControlError(Control control, string message) {
            if (control != null) {
                Bitmap bitmap = Properties.Resources.Image_WarningInfo;
                System.IntPtr iconHandle = bitmap.GetHicon();
                System.Drawing.Icon icon = Icon.FromHandle(iconHandle);
                ErrorProvider errorProvider = new ErrorProvider();
                errorProvider.Icon = icon;
                errorProvider.SetError(control, message);
            }
        }

        #endregion

        /// <summary>
        /// 开窗控件存在于的逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnimotionUIInterceptor_ExecuteReturnExpression(object sender, ExecuteReturnExpressionEventArgs e) {
            GeneralWindowOpener select = sender as GeneralWindowOpener;
            string propertyName = select.ReturnExpressions[0].Left.Replace("ActiveObject.", "");  //第一项绑定的是ID，与当前界面的propertyname对应
            ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c => c.PropertyName == propertyName);
            if (para == null)
                return;

            string str = string.Empty;
            string realValue = string.Empty;
            string display = string.Empty;
            para.ParaValueList.Clear();
            foreach (DependencyObject obj in e.SelectedObjects) {
                if (str == string.Empty) {
                    str = obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_CODE")].ToStringExtension();
                } else
                    str += ";" + obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_CODE")].ToStringExtension();

                if (display == string.Empty) {
                    display = obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_NAME")].ToStringExtension();
                } else
                    display += ";" + obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_NAME")].ToStringExtension();

                para.ParaValueList.Add(new ValueEntity { Value = obj["RETURN_VALUE"], ReturnValue = obj["RETURN_VALUE"], DisplayText = obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_NAME")].ToStringExtension(), HiddenText = obj[para.TargetEntityPirmaryKeyID.Replace("_ID", "_CODE")].ToStringExtension() });
            }

            _uiViewDataSource.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);
            //存在于时，重新设置新的显示值
            _uiViewDataSource[para.PropertyName.Replace("_ID", "_CODE")] = str;
            _uiViewDataSource[para.PropertyName.Replace("_ID", "_NAME")] = display;
            _uiViewDataSource[para.PropertyName + "_RETURN_VALUE"] = str;
            _uiViewDataSource[para.PropertyName] = Maths.GuidDefaultValue();
            _uiViewDataSource.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);

            _dataSource["FORMULA_RESULT"] = CurrentFunction.CalculteFormulaResult();
            e.Handled = true;
        }

    }
}
