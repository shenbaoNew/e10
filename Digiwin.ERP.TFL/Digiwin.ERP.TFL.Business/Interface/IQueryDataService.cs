using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digiwin.Common;
using System.ComponentModel;
using Digiwin.Common.Torridity;

namespace Digiwin.ERP.TFL.Business {
    [TypeKeyOnly]
    [Description("相关查询服务")]
    public interface IQueryDataService {
        DependencyObject QueryCompany(object companyID);

        /// <summary>
        /// 查询计算因子的函数
        /// </summary>
        /// <returns></returns>
        DependencyObjectCollection GetFunctionInfo();
    }
}
