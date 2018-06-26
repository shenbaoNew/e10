//20170829 modi by shenbao for P001-170829002 指标树函数信息支持多语言
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common;
using Digiwin.Common.Torridity;
using Digiwin.Common.Query2;
using Digiwin.Common.Advanced;
using System.Runtime.Remoting.Messaging;
using Digiwin.ERP.TFL.Business.Implement.Properties;

namespace Digiwin.ERP.TFL.Business.Implement {
    [ServiceClass(typeof(IQueryDataService))]
    public sealed class QueryDataService : ServiceComponent, IQueryDataService {
        private Token _token;
        public Token ThisToken {
            get {
                if (_token == null) {
                    _token = CallContext.GetData("Token") as Token;
                }

                return _token;
            }
        }

        #region IQueryDataService 成员

        public DependencyObject QueryCompany(object companyID) {
            QueryNode node = OOQL.Select(OOQL.CreateProperty("COMPANY.COMPANY_ID"),
                    OOQL.CreateProperty("COMPANY.COMPANY_CODE"),
                    OOQL.CreateProperty("COMPANY.COMPANY_NAME")
                )
                .From("COMPANY", "COMPANY")
                .Where(OOQL.AuthFilter("COMPANY", "COMPANY")
                    & OOQL.CreateProperty("COMPANY.COMPANY_ID") == OOQL.CreateConstants(companyID));

            DependencyObjectCollection coll = this.GetService<IQueryService>().ExecuteDependencyObject(node);
            if (coll.Count > 0) {
                return coll[0];
            }

            return null;
        }

        /// <summary>
        /// 查询计算因子的函数
        /// </summary>
        /// <returns></returns>
        public DependencyObjectCollection GetFunctionInfo() {
            SimpleQueryProperty namePro = null;
            SimpleQueryProperty desPro = null;
            SimpleQueryProperty paraName = null; //20170829 add by shenbao for P001-170829002
            //namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_CHS", "FUNCTION_INFO_DISPLAY_NAME"); //20170829 mark by shenbao for P001-170829002
            //desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_CHS", "FUNCTION_EXPLAIN"); //20170829 mark by shenbao for P001-170829002
            //if (ThisToken.CultureInfoName == "en-US") {
            //    namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_ENU", "FUNCTION_INFO_DISPLAY_NAME");
            //    desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_ENU", "FUNCTION_EXPLAIN");
            //} else if (ThisToken.CultureInfoName == "zh-CHT") {
            //    namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_CHT", "FUNCTION_INFO_DISPLAY_NAME");
            //    desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_CHT", "FUNCTION_EXPLAIN");
            //} else {
            //    namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_CHS", "FUNCTION_INFO_DISPLAY_NAME");
            //    desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_CHS", "FUNCTION_EXPLAIN");
            //}

            //20170829 add by shenbao for P001-170829002 ===begin===
            /* 关于这里的Token
             * 由于是在服务端取多语言的字段，不能用缓存的方式，必须用客户端发送过来的实时的token
             * 否则会出现简体客户端先发送请求，后英文客户端登录发请求，token中存储的还是简体语言的情况
             */
            Token token = CallContext.GetData("Token") as Token;
            if (token.CultureInfoName == "en-US") {
                namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_ENU", "FUNCTION_INFO_DISPLAY_NAME");
                desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_ENU", "FUNCTION_EXPLAIN");
                paraName = OOQL.CreateProperty("FUNCTION_INFO_D.PARAMETER_NAME_ENU", "PARAMETER_NAME");
            } else if (token.CultureInfoName == "zh-TW") {
                namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_CHT", "FUNCTION_INFO_DISPLAY_NAME");
                desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_CHT", "FUNCTION_EXPLAIN");
                paraName = OOQL.CreateProperty("FUNCTION_INFO_D.PARAMETER_NAME_CHT", "PARAMETER_NAME"); 
            } else {
                namePro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_DESC_CHS", "FUNCTION_INFO_DISPLAY_NAME");
                desPro = OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_EXPLAIN_CHS", "FUNCTION_EXPLAIN");
                paraName = OOQL.CreateProperty("FUNCTION_INFO_D.PARAMETER_NAME"); 
            }
            //20170829 add by shenbao for P001-170829002 ===end===

            QueryNode node = OOQL.Select(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_NAME"),  //函数编号
                    OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_ID"),
                    OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2", "FUNCTION_INFO_CLASS"),  //供应链类别编号统一加100
                    namePro,  //显示名称
                    desPro,  //描述
                    Formulas.Case(null, OOQL.CreateConstants(Resources.LABEL_000010), new CaseItem[]{
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("101"),OOQL.CreateConstants(Resources.LABEL_000001)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("102"),OOQL.CreateConstants(Resources.LABEL_000002)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("103"),OOQL.CreateConstants(Resources.LABEL_000003)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("104"),OOQL.CreateConstants(Resources.LABEL_000004)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("105"),OOQL.CreateConstants(Resources.LABEL_000005)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("106"),OOQL.CreateConstants(Resources.LABEL_000006)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("107"),OOQL.CreateConstants(Resources.LABEL_000007)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("108"),OOQL.CreateConstants(Resources.LABEL_000008)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("109"),OOQL.CreateConstants(Resources.LABEL_000009)),
                    new CaseItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2")==OOQL.CreateConstants("110"),OOQL.CreateConstants(Resources.LABEL_000010)),
                }, "FUNCTION_INFO_CLASS_NAME"),
                    OOQL.CreateProperty("FUNCTION_INFO_D.FUNCTION_INFO_D_ID"),  //ID
                    OOQL.CreateProperty("FUNCTION_INFO_D.SequenceNumber"),  //序号
                    OOQL.CreateProperty("FUNCTION_INFO_D.PARAMETER_CODE"),  //参数编号
                    paraName,  //参数名称  //20170829 modi by shenbao for P001-170829002
                    OOQL.CreateProperty("FUNCTION_INFO_D.PLACEHOLDER"),  //占位符
                    OOQL.CreateProperty("FUNCTION_INFO_D.PARAMETER_VALUE_TYPE"),  //值类型
                    OOQL.CreateProperty("FUNCTION_INFO_D.CONDITIONS"), //条件
                    OOQL.CreateProperty("FUNCTION_INFO_D.ORGANIZATION_TYPE"),  //组织类型
                    OOQL.CreateProperty("FUNCTION_INFO_D.CONTROL_TYPE"),  //控件类型
                    OOQL.CreateProperty("FUNCTION_INFO_D.CONTROL_CODE"),  //控件绑定编号 如果控件类型是picklist编号为picklistname 如果控件类型是开窗 则编号为开窗的限定方案编号
                    OOQL.CreateProperty("FUNCTION_INFO_D.WINDOW_TYPEKEY")  //开窗TYPEKEY
                )
                .From("FUNCTION_INFO", "FUNCTION_INFO")
                .LeftJoin("FUNCTION_INFO.FUNCTION_INFO_D", "FUNCTION_INFO_D")
                .On(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_ID") == OOQL.CreateProperty("FUNCTION_INFO_D.FUNCTION_INFO_ID"))
                .Where(OOQL.AuthFilter("FUNCTION_INFO", "FUNCTION_INFO")
                    & OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_INFO_STYLE") == OOQL.CreateConstants("1"))
                .OrderBy(OOQL.CreateOrderByItem(OOQL.CreateProperty("FUNCTION_INFO.FUNCTION_CLASS2"), SortType.Asc)
                    , OOQL.CreateOrderByItem(OOQL.CreateProperty("FUNCTION_INFO_D.SequenceNumber"), SortType.Asc));

            return this.GetService<IQueryService>().ExecuteDependencyObject(node);
        }

        #endregion
    }
}