using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common;
using Digiwin.Common.UI;
using System.Windows.Forms;
using Digiwin.Common.Torridity;
using Digiwin.ERP.Common.Utils;

namespace Digiwin.ERP.TFL.UI.Implement
{
    [EventInterceptorClass]
    class ChangeUIControlInterceptor : ServiceComponent {
        [EventInterceptor(typeof(IEditorView), "Load")]
        private void CreateUIControls(object sender, EventArgs e) {
            IFindControlService findControlSrv = this.GetService<IFindControlService>();
            Control control = null;
            if (findControlSrv.TryGet("pickListCOMPANY_TYPE", out control)) {
                if (control != null) {
                    DigiwinPickListLookUpEdit pick = control as DigiwinPickListLookUpEdit;
                    if (pick != null) {
                        ICurrentDocumentWindow win = this.GetServiceForThisTypeKey<ICurrentDocumentWindow>();
                        if (win != null) {
                            string id = win.EditController.EditorView.Id;
                            DependencyObject dataS = win.EditController.Document.DataSource as DependencyObject;
                            if (dataS != null && dataS.ExtendedProperties.Contains("TFL")   //只有在E10中的公式录入界面才执行下面的逻辑，才依据此扩展属性修改过滤条件，在Excel中不控制
                                && (id.Contains("QC") || id.Contains("QM") || id.Contains("FS")
                                || id.Contains("LFS")
                                )) {
                                pick.FilterExpression = "\"[Id] In (2)\"";
                            }

                            if (dataS != null && dataS.ExtendedProperties.Contains("TFL_FormulaView")   //只有在E10中的公式查看按钮才执行下面的逻辑
                                && (id.Contains("QC") || id.Contains("QM") || id.Contains("FS")
                                || id.Contains("LFS")
                                )) {
                                SetFINReadOnly(sender as EditorView);
                            }
                        }
                    }
                }
            }
        }

        private void SetFINReadOnly(Control ctrl) {
            foreach (Control item in ctrl.Controls) {
                if (item is DigiwinSelectControl) {
                    DigiwinSelectControl c = item as DigiwinSelectControl;
                    c.ReadOnly = true;
                } else if (item is DigiwinPickListLookUpEdit) {
                    DigiwinPickListLookUpEdit c = item as DigiwinPickListLookUpEdit;
                    c.Properties.ReadOnly = true;
                } else if (item is DigiwinTextBox) {
                    DigiwinTextBox c = item as DigiwinTextBox;
                    c.ReadOnly = true;
                } else if (item is DigiwinCheckBox) {
                    DigiwinCheckBox c = item as DigiwinCheckBox;
                    c.Enabled = false;
                } else if (item is DigiwinRadioButton) {
                    DigiwinRadioButton c = item as DigiwinRadioButton;
                    c.Enabled = false;
                }

                SetFINReadOnly(item);
            }
        }

        [EventInterceptor(typeof(IEditorView), "DataSourceChanged")]
        private void CreateUIControls22(object sender, EventArgs e) {
            ICurrentDocumentWindow win = this.GetServiceForThisTypeKey<ICurrentDocumentWindow>();
            if (win != null) {
                string id = win.EditController.EditorView.Id;
                DependencyObject dataS = win.EditController.Document.DataSource as DependencyObject;
                if (dataS != null && dataS.ExtendedProperties.Contains("TFL_FormulaView")   //只有在E10中的公式查看按钮才执行下面的逻辑
                    && (id.Contains("QC") || id.Contains("QM") || id.Contains("FS")
                    || id.Contains("LFS")
                    )) {
                    DependencyObject obj = win.EditController.EditorView.DataSource as DependencyObject;

                    bool isLS = false;  //是否存在离散的条件
                    if (obj["FILER_MODE1"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiADMIN_UNIT_CODE"] = obj["uiADMIN_UNIT_NAME"] = obj["ADMIN_UNITS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE6"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiCUSTOMER_CODE"] = obj["uiCUSTOMER_NAME"] = obj["CUSTOMERS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE4"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiSUPPLIER_CODE"] = obj["uiSUPPLIER_NAME"] = obj["SUPPLIERS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE2"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiEMPLOYEE_CODE"] = obj["uiEMPLOYEE_NAME"] = obj["EMPLOYEES"].ToStringExtension();
                    }
                    if (obj["FILER_MODE5"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiSALES_CENTER_CODE"] = obj["uiSALES_CENTER_NAME"] = obj["SALES_CENTERS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE3"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiSUPPLY_CENTER_CODE"] = obj["uiSUPPLY_CENTER_NAME"] = obj["SUPPLY_CENTERS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE7"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiPLANT_CODE"] = obj["uiPLANT_NAME"] = obj["PLANTS"].ToStringExtension();
                    }
                    if (obj["FILER_MODE8"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiAUXILIARY_ITEM_CODE"] = obj["uiAUXILIARY_ITEM_NAME"] = obj["AUXILIARY_ITEM1S"].ToStringExtension();
                    }
                    if (obj["FILER_MODE9"].ToInt32() == 2) {
                        isLS = true;
                        obj["uiAUXILIARY_ITEM_CODE_02"] = obj["uiAUXILIARY_ITEM_NAME_02"] = obj["AUXILIARY_ITEM2S"].ToStringExtension();
                    }

                    if (isLS && !obj.ExtendedProperties.Contains("TFL_IN")) {  //第一次通过业务的入口进去时，要初始化各参数的值
                        obj.ExtendedProperties.Add("TFL_IN", "TFL");
                    }

                    EditorView ev = sender as EditorView;
                    Control[] ctrls = ev.Control.Controls.Find("TabControl1", true);
                    if (ctrls.Length > 0) {
                        TabControl tc = ctrls[0] as TabControl;
                        if (tc != null) {
                            SetFINReadOnly(tc.TabPages["HeaderTab_2"]);  // 公式查看各参数只读
                        }
                    }
                }
            }
        }
    }
}
