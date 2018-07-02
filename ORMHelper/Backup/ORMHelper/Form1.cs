using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Digiwin.Common.Torridity.DataSource;
using Digiwin.Common.Torridity;
using Digiwin.Common;
using Digiwin.Common.Torridity.Metadata;
using Digiwin.Common.Query2;
using Digiwin.Common.Torridity.Query;
using DevExpress.XtraGrid.Views.Grid;

namespace WindowsFormsApplication11 {
    public partial class Form1 : Form {
        private ListBox listbox;
        private ToolStripControlHost listboxHost;
        private ORMHelper _helper;
        public Form1() {
            InitializeComponent();

            listbox = new ListBox();
            listboxHost = new ToolStripControlHost(listbox);
            listbox.Cursor = Cursors.Hand;
            listbox.BorderStyle = BorderStyle.None;
            listbox.HorizontalScrollbar = false;
            listbox.HorizontalExtent = 631;
            listbox.DrawMode = DrawMode.OwnerDrawVariable;
            listbox.ItemHeight = 48;
            listbox.DrawItem += new DrawItemEventHandler(listbox_DrawItem);
            _helper = new ORMHelper();
        }

        private void Form1_Load(object sender, EventArgs e) {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Code",typeof(string));
            DataColumn dc1 = new DataColumn("Name", typeof(string));
            dt.Columns.Add(dc);
            dt.Columns.Add(dc1);
            this.bindingSource1.DataSource = dt;
            this.gridControl1.DataSource = this.bindingSource1;

            this.gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
        }

        private void TextOrm() {
            //_hepler.VeryfiyRepair();

            DependencyObject entity = (DependencyObject)this._helper.Create(Product.DefaultType);
            entity[0] = Guid.NewGuid().ToString();
            entity[1] = "001";
            entity[2] = "CPU";
            //this._helper.Save(entity);

            QueryNode node = OOQL.Select("CODE", "NAME")
                .From("Product")
                .Where(OOQL.CreateProperty("ID") == OOQL.CreateConstants("3DE1E070-241C-42BE-8704-AC46140CCE09"));
            IQueryBuilder builder = OrmEntry.CreateQuery(_helper.DBSource, _helper.DBDriver);
            string str = builder.Select("Product", new string[] { "CODE", "NAME" }, null, null);

            Digiwin.Common.Torridity.Query.QueryEntity queryEn = new Digiwin.Common.Torridity.Query.QueryEntity("Product");

            //DataSet ds = builder.ExecuteSelect(new Digiwin.Common.Torridity.Query.QueryEntity[] { queryEn });

            Product p = new Product(((DependencyObject)Product.DefaultType.CreateInstance()));
            p.ID = Guid.NewGuid();
            p.CODE = "222";
            this._helper.Save(p.DependencyObject);
        }

        //
        // 摘要: 
        //     在所有者绘制的 System.Windows.Forms.ListBox 在视觉外观更改时发生
        void listbox_DrawItem(object sender, DrawItemEventArgs e) {
            
        }
    }

    /// <summary>
    /// ORM辅助工具
    /// </summary>
    public class ORMHelper {
        private const string ConnectionString = "Data Source=.;Initial Catalog=E10_6.0_KF;User ID=sa;pwd=shenbao";
        public IDBSource DBSource { get; private set; }
        public IDBDriver DBDriver { get; private set; }
        public ORMHelper() {
            DBSource = OrmEntry.CreateDBSource();
            //注册主实体定义
            DBSource.DataEntityTypes.Add(Customer.DefaultType);
            DBSource.DataEntityTypes.Add(Product.DefaultType);
            DBSource.DataEntityTypes.Add(Quotation.DefaultType);
            //添加数据库驱动
            this.DBDriver = new SqlServerDriver();
            this.DBDriver.ConnectionString = ConnectionString;
        }
        /// <summary>
        /// 校验并修复数据库
        /// </summary>
        public void VeryfiyRepair() {
            if (this.DBDriver != null && this.DBSource != null) {
                IMetadataTask[] taskes = this.DBDriver.VerifyMetadata(DBSource.DBSetMetadatas.ToArray<IDBSetMetadata>(), OrmOption.CreateDefaultOption());
                this.DBDriver.UpdateMetadata(taskes);
            }
        }
        public object Create(IDataEntityType entityType) {
            IDocumentService service = OrmEntry.CreateDocumentService(entityType, this.DBDriver) as IDocumentService;
            if (service != null) {
                return service.Create();
            }
            return null;
        }
        public object Read(IDataEntityType entityType, object oid) {
            IDocumentService service = OrmEntry.CreateDocumentService(entityType, this.DBDriver) as IDocumentService;
            if (service != null) {
                return service.Read(oid);
            }
            return null;
        }
        public void Delete(IDataEntityType entityType, object oid) {
            IDocumentService service = OrmEntry.CreateDocumentService(entityType, this.DBDriver) as IDocumentService;
            if (service != null) {
                service.Delete(oid);
            }
        }
        public void Save(DependencyObject entity) {
            if (entity == null) {
                throw new ArgumentNullException("entity");
            }
            IDocumentService service = OrmEntry.CreateDocumentService(entity.GetDataEntityType(), this.DBDriver) as IDocumentService;
            if (service != null) {
                service.Save(entity);
            }
        }
    }
}
