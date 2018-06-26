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

namespace Digiwin.ERP.SCM.UI.Implement {
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
        private const int ColumnCount = 5;

        //虚拟绑定实体数据源
        private DependencyObject _dataSource = null;
        
        /// <summary>
        ///  界面行数
        ///  多2行用于界面顶端和低端的占位
        /// </summary>
        public int RowCount {
            get {
                if (CurrentFunction == null || CurrentFunction.ParaList.Count <= 0)
                    return 2;
                return CurrentFunction.ParaList.Count + 2;
            }
        }

        public FunctionEntity CurrentFunction
        {
            get {
                return _funtionList.FirstOrDefault(c => c.Name == _dataSource["Function"].ToStringExtension());
            }
        }

        [EventInterceptor(typeof(IEditorView), "Load")]
        private void CreateUIControls(object sender, EventArgs e) {
            IFindControlService findControlSrv = this.GetService<IFindControlService>();
            Control control = null;
            _funtionList = UIEntity.CreateViewEntity(null);
            DependencyObjectView dv = CreateDataSource();
            _dataSource = dv.DependencyObject;
            _dataSource.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dataSource_PropertyChanged);
            if (findControlSrv.TryGet("desingerTabControl1", out control)) {
                TabControl tabControl = control as TabControl;
                tabControl.SuspendLayout();
                CreateTableLayout();
                CreateUIView();
                tabControl.TabPages[0].Controls.Add(_table);
                tabControl.ResumeLayout(false);
            }
        }

        void btn_Click(object sender, ButtonPressedEventArgs e) {
            using (DocFWOForm form = new DocFWOForm(new Guid(), "", GetPickListDataource("BusinessApproveStatus", true), this.ResourceServiceProvider)) {
                form.ShowDialog();
                ButtonEdit btn = sender as ButtonEdit;
                ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c=>c.Name == btn.Name);
                if (para != null) {
                    _dataSource[para.Name] = form.RtnValues;
                }
            }
        }

        /// <summary>
        /// 创建布局控件
        /// </summary>
        private void CreateTableLayout()
        {
            _table = new TableLayoutPanel();
            _table.Name = "TableLayoutPanel1";
            _table.Dock = DockStyle.Fill;
            _table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            _table.ColumnCount = ColumnCount;
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            _table.RowCount = this.RowCount;
            for (int i = 0; i < this.RowCount; i++)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            }
        }

        /// <summary>
        /// 组建界面
        /// </summary>
        /// <param name="dataSource"></param>
        private void CreateUIView() {
            //创建第一行函数列表
            Label firstLbl = ControlUtils.CreateLabel("lbl", "选择函数", false);
            _table.Controls.Add(firstLbl, 0, 0);
            LookUpEdit lookupFun = ControlUtils.CreateLookUpEdit(_dataSource, "Function", "Function", true, 0, null, 0);
            _table.Controls.Add(lookupFun, 1, 0);

            int index = 1;
            int row = 1;
            foreach (ParaEntity para in CurrentFunction.ParaList) {
                //标签
                Label lbl = ControlUtils.CreateLabel("lbl", para.DisplayName, false);
                _table.Controls.Add(lbl, 0, row);

                //条件选项
                DigiwinPickListLookUpEdit condition = ControlUtils.CreatePickList(_dataSource, para.ConditionName, para.ConditionName, "Condition", false, null, 2);
                _table.Controls.Add(condition, 1, row);

                //参数
                #region 控件类型
                if (para.ParaTypeFlag == ParaTypeEnum.DataTime) {
                    DigiwinDateTimePicker date = ControlUtils.CreateDateTimePicker(_dataSource, para.Name, para.Name, false, index++);
                    _table.Controls.Add(date, 2, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.PickList) {
                    LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_dataSource, para.Name, para.Name, true, 0, null, index++);
                    lookup.Anchor = AnchorStyles.Left;
                    lookup.Width = 150;
                    GetPickDataSource(para.PickListTypeName, lookup);
                    _table.Controls.Add(lookup, 2, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.SelectControl) {
                    DigiwinSelectControl select = ControlUtils.CreateSelectControl(_dataSource, para.Name, para, false, false, index++);
                    select.Anchor = AnchorStyles.Left;
                    select.Width = 150;
                    _table.Controls.Add(select, 2, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.Decimal) {
                    DigiwinTextBox dec = ControlUtils.CreateNumericTextBox(_dataSource, para.Name, para.Name, "0", true, false, 10, index++);
                    dec.Width = 150;
                    _table.Controls.Add(dec, 2, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.String) {
                    DigiwinTextBox str = ControlUtils.CreateNumericTextBox(_dataSource, para.Name, para.Name, "0", false, false, 10, index++);
                    str.Width = 150;
                    _table.Controls.Add(str, 2, row);
                } else if (para.ParaTypeFlag == ParaTypeEnum.Bool) {
                    DigiwinCheckBox chk = ControlUtils.CreateCheckBox(_dataSource, para.Name, para.Name, "", false, true, index++);
                    chk.Width = 50;
                    _table.Controls.Add(chk, 2, row);
                }
                #endregion
                para.Row = row;
                para.Column = 2;

                row++;
            }
        }

        /// <summary>
        /// 创建界面绑定view
        /// </summary>
        /// <returns></returns>
        private DependencyObjectView CreateDataSource() {
            DependencyObjectType type = new DependencyObjectType("Type");
            type.RegisterSimpleProperty("Function", typeof(string));
            foreach (ParaEntity para in CurrentFunction.ParaList) {
                type.RegisterSimpleProperty(para.ConditionName, typeof(string));
                type.RegisterSimpleProperty(para.Name, para.ParaType);
                if (para.ParaTypeFlag == ParaTypeEnum.SelectControl) {
                    type.RegisterSimpleProperty(para.Name.Replace("_ID", "_NAME"), typeof(string));
                    type.RegisterSimpleProperty(para.Name.Replace("_ID", "_CODE"), typeof(string));
                }
            }

            DependencyObject obj = new DependencyObject(type);

            DependencyObjectView view = new DependencyObjectView(obj);

            return view;
        }

        private void RegisterPropertyByFunction()
        { 

        }

        /// <summary>
        /// 创建picklist的datasource
        /// 需要添加空白行
        /// </summary>
        /// <param name="pickListName"></param>
        /// <param name="lookUpEdit"></param>
        /// <returns></returns>
        private DependencyObjectCollection GetPickDataSource(string pickListName,LookUpEdit lookUpEdit) {
            DependencyObjectCollection items = GetPickListDataource(pickListName, false);

            LookUpColumnInfo c1 = new LookUpColumnInfo("Value");
            c1.Visible = false;
            LookUpColumnInfo c2 = new LookUpColumnInfo("DisplayName");

            lookUpEdit.Properties.Columns.Add(c1);
            lookUpEdit.Properties.Columns.Add(c2);

            lookUpEdit.Properties.DataSource = items;
            lookUpEdit.Properties.DisplayMember = "DisplayName";
            lookUpEdit.Properties.ValueMember = "Value";
            lookUpEdit.Properties.NullText = string.Empty;
            lookUpEdit.Properties.TextEditStyle = 0;
            lookUpEdit.Properties.ShowHeader = false;

            return items;
        }

        /// <summary>
        /// 创建函数列表的datasource
        /// 需要添加空白行
        /// </summary>
        /// <param name="pickListName"></param>
        /// <param name="lookUpEdit"></param>
        /// <returns></returns>
        private DependencyObjectCollection GetFunDataSource(string pickListName, LookUpEdit lookUpEdit)
        {
            DependencyObjectCollection items = GetPickListDataource(pickListName, false);

            LookUpColumnInfo c1 = new LookUpColumnInfo("Value");
            c1.Visible = false;
            LookUpColumnInfo c2 = new LookUpColumnInfo("DisplayName");

            lookUpEdit.Properties.Columns.Add(c1);
            lookUpEdit.Properties.Columns.Add(c2);

            lookUpEdit.Properties.DataSource = items;
            lookUpEdit.Properties.DisplayMember = "DisplayName";
            lookUpEdit.Properties.ValueMember = "Value";
            lookUpEdit.Properties.NullText = string.Empty;
            lookUpEdit.Properties.TextEditStyle = 0;
            lookUpEdit.Properties.ShowHeader = false;

            return items;
        }

        private DependencyObjectCollection GetPickListDataource(string pickListName,bool allowCheck) {
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
                first["DisplayName"] = "--空白值--";
            }
            foreach (var item in plDataSrv.GetPickListSortedData(pickListName)) {
                DependencyObject obj = items.AddNew();
                obj["Value"] = item.Id;
                obj["DisplayName"] = item.DisplayName;
                if (allowCheck) {
                    obj["Select"] = false;
                }
            }

            return items;
        }

        void dataSource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            ParaEntity para = CurrentFunction.ParaList.FirstOrDefault(c => c.ConditionName == e.PropertyName);
            if (para == null || para.ParaTypeFlag != ParaTypeEnum.PickList)
                return;
            DependencyObject obj = sender as DependencyObject;
            if (obj[e.PropertyName].ToString() == "8") {
                _table.SuspendLayout();
                Control control = _table.GetControlFromPosition(2, para.Row);

                ButtonEdit btn = ControlUtils.CreateButtonEdit(_dataSource,para.Name,para.Name,false,string.Empty,control.TabIndex);
                btn.ButtonClick += new ButtonPressedEventHandler(btn_Click);
                btn.Width = control.Width;
                _table.Controls.Remove(control);
                _table.Controls.Add(btn, 2, para.Row);
                _table.ResumeLayout(true);
            } else {
                _table.SuspendLayout();
                Control control = _table.GetControlFromPosition(2, para.Row);

                LookUpEdit lookup = ControlUtils.CreateLookUpEdit(_dataSource, para.Name, para.Name, true, 0, null, control.TabIndex);
                lookup.Anchor = AnchorStyles.Left;
                lookup.Width = 150;
                lookup.Name = para.Name;
                GetPickDataSource(para.PickListTypeName, lookup);

                _table.Controls.Remove(control);
                _table.Controls.Add(lookup, 2, para.Row);
                _table.ResumeLayout(true);
            }
        }
    }
}
