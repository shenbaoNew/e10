//20170829 modi by shenbao for P001-170829002 指标树函数信息支持多语言
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common.UI;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using Digiwin.Common.Torridity;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;

namespace Digiwin.ERP.TFL.UI.Implement {
    public class ControlUtils {
        #region 创建控件

        public static Label CreateLabel(string name, string text, bool isReadOnly, int width) {  //20170829 modi by shenbao for P001-170829002 添加宽度
            Label lblProperty = new Label();
            lblProperty.Name = name;
            lblProperty.Text = text;
            lblProperty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            lblProperty.Width = width; //20170829 add by shenbao for P001-170829002

            return lblProperty;
        }

        public static TextBox CreateTextBox(DependencyObject dataSource, string name, string bindingPropertyName, string text
            , bool isReadOnly, bool visible, int maxLength, int tabIndex) {
            TextBox txtSetValue = new TextBox();
            txtSetValue.Name = name;
            txtSetValue.TabIndex = tabIndex;
            txtSetValue.ReadOnly = isReadOnly;
            txtSetValue.MaxLength = maxLength;
            txtSetValue.Visible = visible;

            if (!string.IsNullOrEmpty(bindingPropertyName))  //有绑定时取绑定字段
            {
                txtSetValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", dataSource.DefaultView, bindingPropertyName, true, DataSourceUpdateMode.OnPropertyChanged, string.Empty));
            } else
                txtSetValue.Text = text;  //没有绑定时，赋值显示

            return txtSetValue;
        }

        public static DigiwinTextBox CreateNumericTextBox(DependencyObject dataSource, string name, string bindingPropertyName
            , string text, bool isOnlyNumber
            , bool isReadOnly, bool visible, int maxLength, int tabIndex) {
            DigiwinTextBox txtSetValue = new DigiwinTextBox();
            txtSetValue.Name = name;
            txtSetValue.TabIndex = tabIndex;
            txtSetValue.OnlyNumber = isOnlyNumber;
            txtSetValue.ReadOnly = false;
            txtSetValue.MaxLength = maxLength;
            txtSetValue.Visible = visible;

            if (!string.IsNullOrEmpty(bindingPropertyName)) {
                Binding bind = new System.Windows.Forms.Binding("Text", dataSource.DefaultView, bindingPropertyName, false, DataSourceUpdateMode.OnPropertyChanged, 0);
                txtSetValue.DataBindings.Add(bind);
            } else {
                decimal temp = 0m;
                if (decimal.TryParse(text, out temp))
                    txtSetValue.Text = text;
            }

            return txtSetValue;
        }

        public static DigiwinCheckBox CreateCheckBox(DependencyObject dataSource, string name, string bindingPropertyName, string text, bool isCheck
            , bool isEnabled, bool visible, int tabIndex) {
            DigiwinCheckBox chkSetValue = new DigiwinCheckBox();
            chkSetValue.Name = name;
            chkSetValue.TabIndex = tabIndex;
            chkSetValue.Checked = isCheck;
            chkSetValue.Enabled = isEnabled;
            chkSetValue.Text = text;
            chkSetValue.Visible = visible;

            if (!string.IsNullOrEmpty(bindingPropertyName)) {
                chkSetValue.DataBindings.Add(new System.Windows.Forms.Binding("Checked", dataSource.DefaultView, bindingPropertyName, true, DataSourceUpdateMode.OnPropertyChanged, DBNull.Value));
            } else {
                chkSetValue.Checked = isCheck;
            }

            return chkSetValue;
        }

        public static DigiwinDateTimePicker CreateDateTimePicker(DependencyObject dataSource, string name, string bindingPropertyName
            , bool isReadOnly, bool visible, bool isDateTime, int tabeIndex) {
            DigiwinDateTimePicker dtp = new DigiwinDateTimePicker();
            dtp.Name = name;
            dtp.TabIndex = tabeIndex;
            dtp.ReadOnly = isReadOnly;
            dtp.Visible = visible;
            if (isDateTime) {
                dtp.DateTimeFormat = DateTimePickerFormat.Custom;
                dtp.FormatString = "yyyy/MM/dd HH:mm:ss";
            }

            if (!string.IsNullOrEmpty(bindingPropertyName))
                dtp.DataBindings.Add(new System.Windows.Forms.Binding("Value", dataSource.DefaultView, bindingPropertyName, true, DataSourceUpdateMode.OnPropertyChanged, OrmDataOption.EmptyDateTime));

            return dtp;
        }

        public static GroupBox CreateGroupBox(string name, string text, Point location, int width, int height, bool visible) {
            GroupBox gbox = new GroupBox();
            gbox.Name = name;
            gbox.Location = location;
            gbox.Size = new Size(width, height);
            gbox.Text = text;
            gbox.Visible = visible;

            return gbox;
        }

        public static DigiwinSelectControl CreateSelectControl(DependencyObject dataSource, string name, string propertyName, ParaEntity para
            , bool isReadOnly, bool visible, bool allowMultiSelect, int tabeIndex) {
            DigiwinSelectControl dsc1 = new DigiwinSelectControl();
            ((System.ComponentModel.ISupportInitialize)(dsc1)).BeginInit();
            dsc1.Name = name;
            dsc1.SelectStyle = SelectStyle.TwoInOneOpen;
            dsc1.ReadOnly = isReadOnly;
            dsc1.Visible = visible;
            dsc1.AllowInput = false;
            dsc1.AllowOpenWindowInOpenState = false;
            BindingSource bs = new BindingSource();
            string hiddenText = propertyName.Replace("_ID", "_CODE");
            string displayText = propertyName.Replace("_ID", "_NAME");

            if (dataSource != null) {
                bs.DataSource = dataSource.DefaultView;

                dsc1.DataBindings.Add(new Binding("Value", bs, propertyName, true, DataSourceUpdateMode.OnPropertyChanged, Maths.GuidDefaultValue()));
                dsc1.DataBindings.Add(new Binding("HiddenText", bs, hiddenText, true, DataSourceUpdateMode.OnPropertyChanged));
                dsc1.DataBindings.Add(new Binding("DisplayText", bs, displayText, true, DataSourceUpdateMode.OnPropertyChanged));
            }

            GeneralWindowOpener generalWindowOpener = new GeneralWindowOpener();
            generalWindowOpener.Description = null;
            generalWindowOpener.InexactQuery.QueryProjectId = null;
            generalWindowOpener.InexactQuery.QueryTypeKey = null;
            generalWindowOpener.InexactQuery.ReturnField = null;
            generalWindowOpener.Name = null;
            generalWindowOpener.OpenCondition = null;
            generalWindowOpener.OpenFailMessage = null;
            generalWindowOpener.RelatedType = RelatedType.BindingField;
            generalWindowOpener.MultiSelect = allowMultiSelect;
            //if (propertyVM.OpeningPatameters.Count > 0) {
            //    foreach (OpeningParameter parameter in propertyVM.OpeningPatameters) {
            //        generalWindowOpener.OpeningParameters.Add(parameter);
            //    }
            //}

            ReturnExpression returnExpression1 = new ReturnExpression();
            ReturnExpression returnExpression2 = new ReturnExpression();
            ReturnExpression returnExpression3 = new ReturnExpression();
            ReturnExpression returnExpression4 = new ReturnExpression();

            generalWindowOpener.QueryProjectId = para.QueryProjectId;
            generalWindowOpener.QueryTypeKey = para.QueryTypeKey;

            returnExpression1.Left = "ActiveObject." + propertyName;
            returnExpression1.Name = "ActiveObject." + para.PropertyName;
            returnExpression1.Right = "SelectedObjects[0]." + para.TargetEntityPirmaryKeyID;

            returnExpression2.Left = "ActiveObject." + hiddenText;
            returnExpression2.Name = "ActiveObject." + para.TargetEntityPirmaryKeyID.Replace("_ID", "_CODE");
            returnExpression2.Right = "SelectedObjects[0]." + para.TargetEntityPirmaryKeyID.Replace("_ID", "_CODE");

            returnExpression3.Left = "ActiveObject." + displayText;
            returnExpression3.Name = "ActiveObject." + para.TargetEntityPirmaryKeyID.Replace("_ID", "_NAME");
            returnExpression3.Right = "SelectedObjects[0]." + para.TargetEntityPirmaryKeyID.Replace("_ID", "_NAME");

            returnExpression4.Left = "ActiveObject." + propertyName + "_RETURN_VALUE";
            returnExpression4.Name = "ActiveObject." + para.PropertyName + "_RETURN_VALUE";
            returnExpression4.Right = "SelectedObjects[0].RETURN_VALUE";

            generalWindowOpener.ReturnExpressions.Add(returnExpression1);
            generalWindowOpener.ReturnExpressions.Add(returnExpression2);
            generalWindowOpener.ReturnExpressions.Add(returnExpression3);
            generalWindowOpener.ReturnExpressions.Add(returnExpression4);

            generalWindowOpener.ReturnField = para.TargetEntityPirmaryKeyID;
            generalWindowOpener.Shortcut = System.Windows.Forms.Keys.F2;
            generalWindowOpener.Tip = para.Tip;
            dsc1.WindowOpeners.Add(generalWindowOpener);
            ((System.ComponentModel.ISupportInitialize)(dsc1)).EndInit();

            return dsc1;
        }

        public static LookUpEdit CreateLookUpEdit(DependencyObject dataSource, string name, string propertyName
            , bool isReadOnly, bool visible, int textEditStyle, object defaultValue, int tabeIndex) {
            LookUpEdit pickList = new LookUpEdit();
            pickList.Name = name;
            pickList.Properties.SearchMode = SearchMode.AutoComplete;
            pickList.Properties.TextEditStyle = TextEditStyles.Standard;
            pickList.TabIndex = tabeIndex;
            pickList.Properties.ShowFooter = true;
            pickList.Visible = visible;

            //自适应宽度                 
            //pickList.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            //pickList.ItemIndex = 0;//选择第一项

            //最多显示10个下拉项
            pickList.Properties.PopupResizeMode = ResizeMode.LiveResize;

            if (dataSource != null) {
                pickList.DataBindings.Add(new Binding("EditValue", dataSource.DefaultView, propertyName, true, DataSourceUpdateMode.OnPropertyChanged, defaultValue));
            }
            return pickList;
        }

        public static ButtonEdit CreateButtonEdit(DependencyObject dataSource, string name, string propertyName
            , bool isReadOnly, bool visible, string defaultValue, int tabeIndex) {
            ButtonEdit btn = new ButtonEdit();
            btn.Name = name;
            btn.TabIndex = tabeIndex;
            btn.Visible = visible;

            if (dataSource != null) {
                btn.DataBindings.Add(new Binding("EditValue", dataSource.DefaultView, propertyName, true, DataSourceUpdateMode.OnPropertyChanged));
            }
            return btn;
        }

        public static DigiwinPickListLookUpEdit CreatePickList(DependencyObject dataSource, string name, string propertyName
            , string pickListTypeName
            , bool isReadOnly, bool visible, string defaultValue, int tabeIndex) {
            DigiwinPickListLookUpEdit pickList = new DigiwinPickListLookUpEdit();
            pickList.Name = name;
            pickList.PickListTypeName = pickListTypeName;
            pickList.Properties.SearchMode = SearchMode.AutoComplete;
            pickList.TabIndex = tabeIndex;
            pickList.ItemIndex = 0;
            pickList.Visible = visible;
            if (dataSource != null) {
                pickList.DataBindings.Add(new Binding("EditValue", dataSource.DefaultView, propertyName, true, DataSourceUpdateMode.OnPropertyChanged));
            }
            return pickList;
        }

        #endregion

        public static void BindSimpleCustomDataSource(DependencyObjectCollection colls, LookUpEdit lookUpEdit)
        {
            LookUpColumnInfo c1 = new LookUpColumnInfo("Value");
            c1.Visible = false;
            LookUpColumnInfo c2 = new LookUpColumnInfo("DisplayName");
            lookUpEdit.Properties.Columns.Add(c1);
            lookUpEdit.Properties.Columns.Add(c2);

            lookUpEdit.Properties.DisplayMember = "DisplayName";
            lookUpEdit.Properties.ValueMember = "Value";
            lookUpEdit.Properties.NullText = string.Empty;
            lookUpEdit.Properties.TextEditStyle = 0;
            lookUpEdit.Properties.ShowHeader = false;
            lookUpEdit.Properties.DataSource = colls;
        }
    }
}
