using System.Collections.Generic;
 

namespace WindowsFormsApplication1
{
    /// <summary>
    ///     缓存实体类
    /// </summary>
    public class CacheTaskRunModel
    {
        /// <summary>
        ///     企业编号
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        ///     企业的名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        ///     总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        ///     当前页
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///     商品总条数
        /// </summary>
        public decimal? Sum { get; set; }

        /// <summary>
        /// 添加批准文号改版商品列表
        /// </summary>
        public List<ResponseGoods> GoodsList { get; set; }
      
    }
}