using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class UserFile
    {
        /// <summary>
        /// 文件根路径
        /// </summary>
        public static string RootPath
        {
            get
            {
                return Config.FilePath.IsNullOrWhiteSpace() ? Tools.GetContentRootPath() : Config.FilePath;
            }
        }
        /// <summary>
        /// 得到用户根目录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserRoot(Guid userId)
        {
            string path = RootPath + "/UserFiles/" + userId.ToUpperString() + "/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 得到用户目录JSON
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="directory">路径，默认为用户根目录</param>
        /// <param name="isSelect">是否是选择，如果是选择，不显示我的分享文件，我收到的文件等</param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetUserDirectoryJSON(Guid userId, string directory = "", bool isSelect = false, IStringLocalizer localizer = null)
        {
            string path = directory.IsNullOrWhiteSpace() ? GetUserRoot(userId) : directory;
            if (!HasAccess(path, userId))
            {
                return "[]";
            }
            var dirs = Directory.EnumerateDirectories(path);
            string rootId = path.DESEncrypt();
            JArray jArrayChild = new JArray();
            foreach (var dir in dirs)
            {
                bool hasChilds = Directory.EnumerateDirectories(dir).Any();
                JObject dirObject = new JObject
                {
                    { "id", dir.DESEncrypt() },
                    { "parentID", rootId },
                    { "title", Path.GetFileName(dir) },
                    { "type", 1 },
                    { "ico", hasChilds ? "" : "fa-folder" },
                    { "hasChilds", hasChilds ? 1 : 0 }
                };
                jArrayChild.Add(dirObject);
            }
            if (!directory.IsNullOrWhiteSpace())
            {
                return jArrayChild.ToString(Newtonsoft.Json.Formatting.None);
            }
            JArray jArray = new JArray();
            JObject rootObject = new JObject
            {
                { "id", rootId },
                { "parentID", 0 },
                { "title", localizer == null ? "我的文件" : localizer["MyFile"]},
                { "type", 0 },
                { "ico", "fa-hdd-o" },
                { "hasChilds", dirs.Any() ? 1 : 0 }
            };
            jArray.Add(rootObject);
            if (!isSelect)
            {
                JObject sharObject = new JObject
                {
                    { "id", "myshare" },
                    { "parentID", 0 },
                    { "title", localizer == null ? "我分享的文件" : localizer["MyShareFile"] },
                    { "type", 0 },
                    { "ico", "fa-share-alt-square" },
                    { "hasChilds", 0 }
                };
                jArray.Add(sharObject);
                JObject sharObject1 = new JObject
                {
                    { "id", "sharemy" },
                    { "parentID", 0 },
                    { "title", localizer == null ? "我收到的文件" : localizer["ShareMyFile"] },
                    { "type", 0 },
                    { "ico", "fa-external-link-square" },
                    { "hasChilds", 0 }
                };
                jArray.Add(sharObject1);
            }
            rootObject.Add("childs", jArrayChild);
            return jArray.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// 得到一个路径下的子目录和文件
        /// </summary>
        /// <param name="directory">目录</param>
        /// <param name="userId">用户ID，如果传了用户ID就只能看自己的目录</param>
        /// <param name="searchPattern">搜索字符串</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式</param>
        /// <returns>(名称, 完整名称, 修改时间, 类型, 大小)</returns>
        public List<(string, string, DateTime, int, long)> GetSubDirectoryAndFiles(string directory, Guid userId, string searchPattern = "", string order = "", int orderType = 0)
        {
            List<(string, string, DateTime, int, long)> ps = new List<(string, string, DateTime, int, long)>();
            if (!HasAccess(directory, userId))
            {
                return ps;
            }
            JArray jArray = new JArray();
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
            {
                return ps;
            }
            //如果关键字不带星号，则要加上星号，不然搜索不到内容
            if (!searchPattern.IsNullOrEmpty() && !searchPattern.Contains('*'))
            {
                searchPattern = "*" + searchPattern + "*";
            }
            var dirs = searchPattern.IsNullOrEmpty() ? directoryInfo.EnumerateDirectories() : directoryInfo.EnumerateDirectories(searchPattern, SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                ps.Add((dir.Name, dir.FullName, dir.LastWriteTime, 0, 0));
            }
            var files = searchPattern.IsNullOrEmpty() ? directoryInfo.EnumerateFiles() : directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                ps.Add((file.Name, file.FullName, file.LastWriteTime, 1, file.Length));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                switch (order.ToLower())
                {
                    case "name":
                        ps = orderType == 0 ? ps.OrderBy(p => p.Item1).ToList() : ps.OrderByDescending(p => p.Item1).ToList();
                        break;
                    case "date":
                        ps = orderType == 0 ? ps.OrderBy(p => p.Item3).ToList() : ps.OrderByDescending(p => p.Item3).ToList();
                        break;
                    case "type":
                        ps = orderType == 0 ? ps.OrderBy(p => p.Item4).ToList() : ps.OrderByDescending(p => p.Item4).ToList();
                        break;
                    case "size":
                        ps = orderType == 0 ? ps.OrderBy(p => p.Item5).ToList() : ps.OrderByDescending(p => p.Item5).ToList();
                        break;
                }
            }
            return ps;
        }

        /// <summary>
        /// 判断当前路径是否可以访问
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userId">用户ID，如果不是空GUID要判断只能访问自己的文件夹</param>
        /// <returns></returns>
        public static bool HasAccess(string path, Guid userId)
        {
            string accessPath = (Config.FilePath.IsNullOrWhiteSpace() 
                ? Tools.GetContentRootPath() : Config.FilePath).Replace("/", "").Replace("\\", "");
            string path1 = path.Replace("/", "").Replace("\\", "");
            bool access = path1.StartsWith(accessPath, StringComparison.CurrentCultureIgnoreCase);
            if (userId.IsNotEmptyGuid())
            {
                access = path1.StartsWith(accessPath+ "UserFiles"+userId.ToUpperString(), StringComparison.CurrentCultureIgnoreCase) ||
                    path1.StartsWith(accessPath + "Attachment" + userId.ToUpperString(), StringComparison.CurrentCultureIgnoreCase);
            }
            return access;
        }

        /// <summary>
        /// 根据完整路径得到相对路径，去掉设置的附件路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string fullPath)
        {
            if (fullPath.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string rootPath = RootPath;
            string relativePath = fullPath.Replace("\\", "/").TrimStart(rootPath.Replace("\\", "/").ToCharArray()).Replace("\\", "/").Trim();
            return relativePath.StartsWith("/") ? relativePath : "/" + relativePath;
        }

        /// <summary>
        /// 得到连接地址
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public static string GetLinkPath(string fullPath, Guid userId, string query, IStringLocalizer localizer = null)
        {
            string rootPath = RootPath + "/UserFiles";
            string relativePath = fullPath.Replace("\\", "/").TrimStart(rootPath.Replace("\\", "/").ToCharArray()).Replace("\\", "/").Trim();
            string[] relativePathArray = relativePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            for (int i = 0; i < relativePathArray.Length; i++)
            {
                string path = relativePathArray[i];
                if (path.IsNullOrWhiteSpace())
                {
                    continue;
                }
                string title = path.Equals(userId.ToUpperString()) ? (localizer == null ? "我的文件" : localizer["MyFile"]) : path;
                stringBuilder1.Append("/" + path);
                string id = (rootPath + stringBuilder1.ToString()).DESEncrypt();
                stringBuilder.Append("<a class=\"blue1\" href=\"List?id=" + id + "&" + query + "\">" + title + "</a>");
                if (i < relativePathArray.Length - 1)
                {
                    stringBuilder.Append("<i class=\"fa fa-angle-right\"></i>");
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到分享文件夹的连接地址
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="dirPath">分享的目录</param>
        /// <returns></returns>
        public static string GetLinkShparePath(string fullPath, string dirPath, string query)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var dirPath1 = dirInfo.Parent.FullName;
            string relativePath = fullPath.Replace("\\", "/").TrimStart(dirPath1.Replace("\\", "/").ToCharArray()).Replace("\\", "/").Trim();
            string[] relativePathArray = relativePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            for (int i = 0; i < relativePathArray.Length; i++)
            {
                string path = relativePathArray[i];
                if (path.IsNullOrWhiteSpace())
                {
                    continue;
                }
                //string title = path.Equals(dirInfo.Name) ? "我的文件" : path;
                stringBuilder1.Append("/" + path);
                string id = (dirPath1 + stringBuilder1.ToString()).DESEncrypt();
                stringBuilder.Append("<a class=\"blue1\" href=\"ShareDirList?id=" + id + "&" + query + "\">" + path + "</a>");
                if (i < relativePathArray.Length - 1)
                {
                    stringBuilder.Append("<i class=\"fa fa-angle-right\"></i>");
                }
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// 重命名文件或文件夹
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <param name="newName">新的文件名</param>
        /// <returns></returns>
        public static string ReName(string path, string newName, IStringLocalizer localizer = null)
        {
            if (!File.Exists(path))
            {
                if (!Directory.Exists(path))
                {
                    return localizer == null ? "文件或目录不存在!" : localizer["NotExistsFileOrFolder"];
                }
                else //如果是目录
                {
                    try
                    {
                        Directory.Move(path, Path.Combine(Directory.GetParent(path).FullName, newName));
                    }
                    catch (IOException err)
                    {
                        return err.Message;
                    }
                    return "1";
                }
            }
            try
            {
                File.Move(path, Path.Combine(Path.GetDirectoryName(path), newName + Path.GetExtension(path)));
            }
            catch (IOException err)
            {
                return err.Message;
            }
            return "1";
        }

        /// <summary>
        /// 移到文件或文件夹
        /// </summary>
        /// <param name="path">多个文件或文件夹</param>
        /// <param name="newPath">要移动到的路径</param>
        /// <returns></returns>
        public static string MoveTo(string[] paths, string newPath, IStringLocalizer localizer = null)
        {
            if (!Directory.Exists(newPath))
            {
                return localizer == null ? "没有找到要移动到的文件夹!" : localizer["NoMoveFolder"];
            }
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                foreach (string path in paths)
                {
                    string toPath = Path.Combine(newPath, Path.GetFileName(path));
                    if (Directory.Exists(path))
                    {
                        if (!Directory.Exists(toPath))
                        {
                            Directory.Move(path, toPath);
                        }
                        else
                        {
                            stringBuilder.Append((localizer == null ? "文件夹" : localizer["Folder"]) + Path.GetFileName(path) + (localizer == null ? "已经存在，" : localizer["Exists"]));
                        }
                    }
                    else if (File.Exists(path))
                    {
                        if (!File.Exists(toPath))
                        {
                            File.Move(path, toPath);
                        }
                        else
                        {
                            stringBuilder.Append((localizer == null ? "文件" : localizer["File"]) + Path.GetFileName(path) + (localizer == null ? "已经存在，" : localizer["Exists"]));
                        }
                    }
                }
            }
            catch (IOException err)
            {
                return err.Message;
            }
            return stringBuilder.Length ==0 ? "1" : stringBuilder.ToString().TrimEnd('，') + "!";
        }
    }
}
