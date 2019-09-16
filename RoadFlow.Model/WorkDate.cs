using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_WorkDate")]
    public class WorkDate
    {
        /// <summary>
        /// WorkDay
        /// </summary>
        [DisplayName("WorkDay")]
        [Key]
        public DateTime WorkDay { get; set; }

        /// <summary>
        /// 0节假日 1工作日
        /// </summary>
        public int IsWork { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
