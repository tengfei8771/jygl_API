using System;
using System.Collections.Generic;
using System.Text;

namespace UIDP.BIZModule.jyglModel
{
    public class CLBXModel
    {
        public string CLBH { get; set; }
        public string DWBM { get; set; }
        public string CCXM { get; set; }
        public string CCSY { get; set; }
        public DateTime CCKSSJ { get; set; }
        public DateTime CCJSSJ { get; set; }
        public decimal CCTS { get; set; }
        public decimal HJJE { get; set; }
        public string HJDX { get; set; }
        public decimal YJCLF { get; set; }
        public decimal YTBJE { get; set; }
        public string REMARK { get; set; }
        public string SKRXM { get; set; }
        public int MyProperty { get; set; }
        public string CJR { get; set; }
        public DateTime CJSJ { get; set; }
        public string BJR { get; set; }
        public DateTime BJSJ { get; set; }
        public int IS_DELETE { get; set; }
        public string XMBH { get; set; }
        public string XMMC { get; set; }
        public string DWMC { get; set; }
        public List<CLXCModel> XCList { get; set; }
    }
}
