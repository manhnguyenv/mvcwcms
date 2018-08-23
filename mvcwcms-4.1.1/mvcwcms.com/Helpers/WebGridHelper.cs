using HtmlAgilityPack;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.WebPages;

namespace MVCwCMS.Helpers
{
    public enum DataHideValue
    {
        All,
        Phone,
        Phone_Tablet,
        Tablet
    }

    public class WebGridColumnHelper : WebGridColumn
    {
        public string ThAttributes { get; set; }
        public DataHideValue? DataHide { get; set; }

        public WebGridColumnHelper()
            : base()
        {
        }
    }

    public class WebGridHelper : WebGrid
    {
        public WebGridHelper(
            IEnumerable<dynamic> source = null,
            IEnumerable<string> columnNames = null,
            string defaultSort = null,
            SortDirection sortDirection = SortDirection.Ascending,
            int rowsPerPage = 10,
            bool canPage = true,
            bool canSort = true,
            string ajaxUpdateContainerId = null,
            string ajaxUpdateCallback = null,
            string fieldNamePrefix = null,
            string pageFieldName = null,
            string selectionFieldName = null,
            string sortFieldName = null,
            string sortDirectionFieldName = null)
            : base(source, columnNames, defaultSort, rowsPerPage, canPage, canSort, ajaxUpdateContainerId, ajaxUpdateCallback, fieldNamePrefix, pageFieldName, selectionFieldName, sortFieldName, sortDirectionFieldName)
        {
            if (sortDirection == SortDirection.Descending && HttpContext.Current.Request.QueryString[base.SortDirectionFieldName].IsEmptyOrWhiteSpace())
            {
                base.SortDirection = SortDirection.Descending;
            }
        }

        public WebGridColumnHelper ExtendedColumn(string columnName = null, string header = null, Func<dynamic, object> format = null, string style = null, bool canSort = true, string thAttributes = null, DataHideValue? dataHide = null)
        {
            WebGridColumnHelper result = new WebGridColumnHelper();
            result.ColumnName = columnName;
            result.Header = header;
            result.Format = format;
            result.Style = style;
            result.CanSort = canSort;
            result.ThAttributes = thAttributes;
            result.DataHide = dataHide;
            return result;
        }

        public WebGridColumnHelper[] ExtendedColumns(params WebGridColumnHelper[] ColumnSet)
        {
            return ColumnSet.ToArray<WebGridColumnHelper>();
        }

        public HelperResult GetExtendedHtml(
            string tableStyle = "table table-striped table-bordered table-hover footable toggle-square",
            string headerStyle = "webgrid-header",
            string footerStyle = "webgrid-footer",
            string rowStyle = null,
            string alternatingRowStyle = null,
            string selectedRowStyle = null,
            string caption = null,
            bool displayHeader = true,
            bool fillEmptyRows = false,
            string emptyRowCellValue = null,
            IEnumerable<WebGridColumnHelper> columns = null,
            IEnumerable<string> exclusions = null,
            WebGridPagerModes mode = WebGridPagerModes.Numeric | WebGridPagerModes.NextPrevious,
            string firstText = null,
            string previousText = null,
            string nextText = null,
            string lastText = null,
            int numericLinksCount = 5,
            Object htmlAttributes = null,
            bool displayTotalItems = true,
            string totalItemsText = "Total items")
        {
            HtmlString result;

            AdminPages adminPages = new AdminPages();
            AdminPage adminPage = adminPages.GetPageByCurrentAction();
            if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Read))
            {
                WebGrid webGrid = this;
                IHtmlString webGridHtml = webGrid.GetHtml(tableStyle, headerStyle, footerStyle, rowStyle, alternatingRowStyle, selectedRowStyle, caption, displayHeader, fillEmptyRows, emptyRowCellValue, columns, exclusions, mode, firstText, previousText, nextText, lastText, numericLinksCount, htmlAttributes);

                string webGridHtmlString = webGridHtml.ToString();

                HtmlDocument htmlDocument = new HtmlDocument();

                //TH Attributes
                htmlDocument.LoadHtml(webGridHtmlString);
                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectSingleNode("//thead/tr").SelectNodes("th");
                int i = 0;
                foreach (WebGridColumnHelper c in columns)
                {
                    if (c.ThAttributes.IsNotNull())
                    {
                        HtmlNode htmlNodeTh = HtmlNode.CreateNode(htmlNodeCollection[i].OuterHtml.Insert(3, " " + c.ThAttributes + " "));
                        htmlNodeCollection[i].ParentNode.ReplaceChild(htmlNodeTh, htmlNodeCollection[i]);
                    }
                    if (c.DataHide.IsNotNull())
                    {
                        HtmlNode htmlNodeTh = HtmlNode.CreateNode(htmlNodeCollection[i].OuterHtml.Insert(3, " data-hide=\"" + c.DataHide.ToString().ToLower().Split('_').ToCSV(',') + "\" "));
                        htmlNodeCollection[i].ParentNode.ReplaceChild(htmlNodeTh, htmlNodeCollection[i]);
                    }
                    i++;
                }
                webGridHtmlString = htmlDocument.DocumentNode.OuterHtml;

                //Sort icon
                if (webGrid.SortColumn.IsNotEmptyOrWhiteSpace())
                {
                    htmlDocument.LoadHtml(webGridHtmlString);
                    HtmlNode htmlNodeAnchor = htmlDocument.DocumentNode.SelectSingleNode("//a[contains(@href,'sort=" + webGrid.SortColumn + "')]");
                    if (htmlNodeAnchor != null)
                    {
                        string imgSortDirection;
                        if (webGrid.SortDirection == SortDirection.Ascending)
                            imgSortDirection = "imgSortDirectionASC";
                        else
                            imgSortDirection = "imgSortDirectionDESC";
                        HtmlNode htmlNodeIcon = HtmlNode.CreateNode("<div class=\"" + imgSortDirection + "\"></div>");

                        htmlNodeAnchor.ParentNode.AppendChild(htmlNodeIcon);

                        // Fix a bug http://stackoverflow.com/questions/759355/image-tag-not-closing-with-htmlagilitypack
                        if (HtmlNode.ElementsFlags.ContainsKey("img"))
                            HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
                        else
                            HtmlNode.ElementsFlags.Add("img", HtmlElementFlag.Closed);

                        webGridHtmlString = htmlDocument.DocumentNode.OuterHtml;
                    }
                }

                //Total Row Count
                htmlDocument.LoadHtml(webGridHtmlString);
                HtmlNode htmlNodeTFoot = htmlDocument.DocumentNode.SelectSingleNode("//tfoot/tr/td");
                if (htmlNodeTFoot != null)
                {
                    string pager = webGrid.Pager(numericLinksCount: 10, mode: WebGridPagerModes.All).ToString();
                    if (displayTotalItems)
                        pager = "<span class=\"pager-total-items-text\">" + totalItemsText + ":</span> <span class=\"pager-total-items-value\">" + webGrid.TotalRowCount.ToString() + "</span><span class=\"pager-pagination\">" + pager + "</span>";

                    htmlNodeTFoot.InnerHtml = pager;

                    // Fix a bug http://stackoverflow.com/questions/759355/image-tag-not-closing-with-htmlagilitypack
                    if (HtmlNode.ElementsFlags.ContainsKey("img"))
                        HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
                    else
                        HtmlNode.ElementsFlags.Add("img", HtmlElementFlag.Closed);

                    webGridHtmlString = htmlDocument.DocumentNode.OuterHtml;
                }

                result = new HtmlString(webGridHtmlString);
            }
            else
            {
                result = new HtmlString("<span class=\"label label-danger\">" + Resources.Strings.InsufficientPermissions + "</span>");
            }

            return new HelperResult(writer =>
            {
                writer.Write(result);
            });
        }
    }
}
