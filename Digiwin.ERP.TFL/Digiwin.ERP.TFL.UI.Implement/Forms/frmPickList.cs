//---------------------------------------------------------------- 
//Copyright (C) 2005-2006 Digital China Management System Co.,Ltd
//Http://www.Dcms.com.cn 
// All rights reserved.
//<author>wangyq</author>
//<createDate>2012-06-07</createDate>
//<description>单据性质开窗窗体</description>

using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Digiwin.Common;
using Digiwin.Common.Advanced;
using Digiwin.Common.Torridity;
using Digiwin.Common.Torridity.Query;
using Digiwin.Common.UI;
using Digiwin.ERP.Common.Utils;

namespace Digiwin.ERP.TFL.UI.Implement {
    public partial class DocFWOForm :DigiwinForm {
        private string _rtnValues = string.Empty;
        public string RtnValues {
            get {
                return _rtnValues;
            }
        }

        public DocFWOForm(object ownerOrgROid, string docTypeName, DependencyObjectCollection colls, IResourceServiceProvider provider) {
            InitializeComponent();
            gridControl1.DataSource = colls;
        }

        /// <summary>
        /// 双击回填
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grid_MouseDoubleClick(object sender, MouseEventArgs e) {
            DependencyObjectCollection coll = this.gridControl1.DataSource as DependencyObjectCollection;
            if (coll != null) {
                GridView gridView = (GridView)(((GridControl)(sender)).FocusedView);
                if (e.Button == MouseButtons.Left && e.Clicks == 2 && gridView.FocusedRowHandle >= 0) {
                    object focusedRow = gridView.GetFocusedRow();
                    if (focusedRow != null) {
                        foreach (DependencyObject item in coll) {
                            if (item["Select"].ToBoolean()) {
                                if (_rtnValues == string.Empty)
                                    _rtnValues = item["Value"].ToStringExtension();
                                else
                                    _rtnValues += ";" + item["Value"].ToStringExtension();
                            }
                        }
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 查找开窗上的gridcontrol
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        private GridControl FindGridControl(Control parentControl) {
            GridControl grdControl = null;
            foreach (Control controlObj in parentControl.Controls) {
                if (controlObj.GetType().Name == typeof(GridControl).Name) {
                    grdControl = (GridControl)controlObj;
                    break;
                }
            }
            if (grdControl == null) {
                foreach (Control controlObj in parentControl.Controls) {
                    grdControl = FindGridControl(controlObj);
                    if (grdControl != null) {
                        break;
                    }
                }
            }
            return grdControl;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancle_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e) {
            DependencyObjectCollection coll = this.gridControl1.DataSource as DependencyObjectCollection;
            if (coll != null) {
                foreach (DependencyObject item in coll) {
                    if (item["Select"].ToBoolean()) {
                        if (_rtnValues == string.Empty)
                            _rtnValues = item["Value"].ToStringExtension();
                        else
                            _rtnValues += ";" + item["Value"].ToStringExtension();
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
