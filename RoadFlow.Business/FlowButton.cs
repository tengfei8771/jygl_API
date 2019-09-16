using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RoadFlow.Business
{
    public class FlowButton
    {
        private readonly Data.FlowButton flowButtonData;
        public FlowButton()
        {
            flowButtonData = new Data.FlowButton();
        }
        /// <summary>
        /// 得到所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowButton> GetAll()
        {
            return flowButtonData.GetAll();
        }
        /// <summary>
        /// 得到一个按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowButton Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个按钮
        /// </summary>
        /// <param name="flowButton">按钮实体</param>
        /// <returns></returns>
        public int Add(Model.FlowButton flowButton)
        {
            return flowButtonData.Add(flowButton);
        }
        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="flowButton">按钮实体</param>
        public int Update(Model.FlowButton flowButton)
        {
            return flowButtonData.Update(flowButton);
        }
        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="flowButtons">按钮实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowButton[] flowButtons)
        {
            return flowButtonData.Delete(flowButtons);
        }
        /// <summary>
        /// 得到最大序号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var all = GetAll();
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }

        /// <summary>
        /// 得到按钮显示标题（多语言时）
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="flowButton">按钮实体</param>
        /// <param name="note">输出备注</param>
        /// <returns></returns>
        public string GetLanguageTitle(Model.FlowButton flowButton, out string note, string language = "zh-CN")
        {
            note = string.Empty;
            if(null == flowButton)
            {
                return string.Empty;
            }
            string title = string.Empty;
            switch (language)
            {
                case "en-US":
                    title = flowButton.Title_en;
                    note = flowButton.Note_en;
                    break;
                case "zh":
                    title = flowButton.Title_zh;
                    note = flowButton.Note_zh;
                    break;
                default:
                    title = flowButton.Title;
                    note = flowButton.Note;
                    break;
            }
            return title;
        }
    }
}
