using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Cache
{
    /// <summary>
    /// 缓存工厂类
    /// </summary>
    internal class Factory
    {
        public static ICache GetInstance()
        {
            //使用系统自带缓存
            return new CoreCache();
        }
    }
}
