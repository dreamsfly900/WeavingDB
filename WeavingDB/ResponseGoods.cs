using System;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    /// <summary>
    ///     api返回的商品信息
    /// </summary>
    public class ResponseGoods
    {
        /// <summary>
        ///     商品Id
        /// </summary>
        public string spid { get; set; }

        /// <summary>
        ///     助记码
        /// </summary>
        public string zjm { get; set; }

        /// <summary>
        ///     商品条码
        /// </summary>
        public string sptm { get; set; }

        /// <summary>
        ///     商品名称
        /// </summary>
        public string spmch { get; set; }

        /// <summary>
        ///     商品规格
        /// </summary>
        public string shpgg { get; set; }

        /// <summary>
        ///     单位
        /// </summary>
        public string dw { get; set; }

        /// <summary>
        ///     生产厂商
        /// </summary>
        public string shpchd { get; set; }

        /// <summary>
        ///     剂型
        /// </summary>
        public string jixing { get; set; }

        /// <summary>
        ///     批准文号
        /// </summary>
        public string pizhwh { get; set; }

        /// <summary>
        ///     库存数量
        /// </summary>
        public float? kcshl { get; set; }

        public string sxrq { get; set; }

        /// <summary>
        ///     大包装
        /// </summary>
        public int? jlgg { get; set; }

        /// <summary>
        ///     中包装
        /// </summary>
        public float? zbz { get; set; }

        /// <summary>
        ///     存储条件
        /// </summary>
        public string cunchtj { get; set; }

        /// <summary>
        ///     有效期
        /// </summary>
        public string youxq { get; set; }

        /// <summary>
        ///     价格
        /// </summary>
        public float? zdshj { get; set; }

        public int rowindex { get; set; }

        /// <summary>
        ///     类别
        /// </summary>
        public string leibie { get; set; }

        /// <summary>
        ///     企业Id
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        ///     商品图片
        /// </summary>
        public string SpPicExtend { get; set; }

        private DateTime? _lastUpdate;

        /// <summary>
        ///     最后更新日期
        /// </summary>
        public DateTime? LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value ?? DateTime.Now; }
        }

        /// <summary>
        ///     商品分类
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        ///     商品图片
        /// </summary>
        public List<string> ImgSrc { get; set; }


        /// <summary>
        /// 判断商品是否失效1-商品失效
        /// </summary>
        public int? IsInvalid { get; set; }

        /// <summary>
        ///     匹配商业公司的标识
        /// </summary>
        public string MatchSpid { get; set; }

        /// <summary>
        ///     商品议价
        /// </summary>
        public decimal? Bargain { get; set; }


        /// <summary>
        ///  共享企业标识
        /// </summary>
        public string ShareOrgId { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string pihao { get; set; }

        /// <summary>
        /// 上架下架状态
        ///  null 是上架
        ///  99 是下架
        /// </summary>
        public int? ShelfStatus { get; set; }

        /// <summary>
        ///  ERP分类
        ///  西药：XY  器械：QX 中药：ZY  保健品：BJP  食品：SP 冷藏：LC 
        /// 10-西药
        /// 20-中药
        /// 30-器械
        /// 40-保健品
        /// 50-食品
        /// 60-冷藏
        /// 70-妊娠药品
        /// 80-特殊药品
        /// </summary>
        public int? Classify { get; set; }

        /// <summary>
        ///  业务员编号
        /// <remarks>
        ///   商品的Spid + 批号 + 业务员编号 确定一个品种
        ///   业务员对应的品种不同
        /// </remarks>
        /// </summary>
        public string SalesmanNo { get; set; }

    }
}