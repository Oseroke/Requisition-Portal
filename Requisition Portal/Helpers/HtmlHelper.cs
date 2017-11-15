using Requisition_Portal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Requisition_Portal.Helpers
{
    public static class MyExtensionMethods
    {


        /// <summary>
        /// Custom modal pop-up control
        /// </summary>
        /// <param name="html">the current Html helper</param>
        /// <param name="popData">The PopupModel class that defines all the modal's properties</param>
        /// <returns></returns>
        public static IDisposable BeginPopupModal(this HtmlHelper html, PopupModel popData)
        {

            var writer = html.ViewContext.Writer;

            writer.Write("<div id='" + popData.ID + "' style='display: none;'>");
            writer.Write("<div class='modal-content messageDialog-msgText'></div>");
            writer.Write("<form id='modal-form'>");
            writer.Write("<div class='modal-internal-content'></div>");

            //----------------------------------------------------
            //internal content will now be "injected" after this
            //---------------------------------------------------

            return new ModalBlock(html, writer, popData);
        }


        private class ModalBlock : IDisposable
        {
            private readonly TextWriter _writer;
            private readonly HtmlHelper _htmlHeper;
            private readonly PopupModel _popData;

            public ModalBlock(HtmlHelper htmlHelper, TextWriter writer, PopupModel popData)
            {
                _htmlHeper = htmlHelper;
                _writer = writer;
                _popData = popData;
            }

            public void Dispose()
            {
                //---------------------------------------------------------
                //internal content has now been "injected" just before this
                //---------------------------------------------------------

                _writer.Write("</form>");
                _writer.Write("</div>");

                //render the ModalPopup view to ensure that PopupModel data binds to the view 
                //it is eaier to use this helper that to actualy write out all the HTML within the C# code
                //Explicity set the full path as this control can be reused in many places
                var partialHTML = _htmlHeper.Partial("~/Views/Shared/ModalPopup.cshtml", _popData);

                _writer.Write(partialHTML);
            }
        }


    }
}