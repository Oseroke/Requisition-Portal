using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RequisitionPortal.Models
{
    /// <summary>
    /// Modal button states
    /// </summary>
    public enum PopupButtonSet
    {
        Ok = 0,             // just close
        Close = 1,          // just close
        OkPostback = 2,     // call something 
        OkPostbackClose = 3 // call something and then close
    }

    public class PopupModel
    {
        /// <summary>
        /// Instantiate the PopupModel
        /// </summary>
        /// <param name="id">The client ID that will be used to identity the popup div</param>
        public PopupModel(string id)
        {
            ID = id;
        }

        public string TitleImageUrl
        {
            private get;
            set;
        }

        public string ID
        {
            get;
            private set;
        }

        private string _title = "Title";
        /// <summary>
        /// The modal pop-up's title
        /// </summary>
        public string Title
        {
            get
            {
                return HttpUtility.HtmlEncode(_title);
            }
            set { _title = value; }
        }

        public PopupButtonSet PopupButtonSet
        {
            get;
            set;
        }

        /// <summary>
        /// Renders the modal pop-up's HTML title image
        /// </summary>
        public string ImageHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(TitleImageUrl))
                {
                    Image image = new Image();
                    image.ImageUrl = VirtualPathUtility.ToAbsolute(TitleImageUrl);
                    image.Width = new Unit(25, UnitType.Pixel);
                    image.Height = new Unit(25, UnitType.Pixel);

                    using (TextWriter swriter = new StringWriter())
                    {
                        HtmlTextWriter hwriter = new HtmlTextWriter(swriter);
                        image.RenderControl(hwriter);
                        return swriter.ToString();
                    }
                }
                else
                    return string.Empty;
            }
        }

        private int _height = 260;
        /// <summary>
        /// The height of the modal
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }

        }

        private int _width = 480;
        /// <summary>
        /// The width of the modal
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }

        }

        private bool _draggable = true;
        /// <summary>
        /// Is the modal draggale
        /// </summary>
        public bool Draggable
        {
            get
            {
                return _draggable;
            }
            set
            {
                _draggable = value;
            }

        }

        private bool _resizable = false;
        /// <summary>
        /// Id the modal resizable
        /// </summary>
        public bool Resizable
        {
            get
            {
                return _resizable;
            }
            set
            {
                _resizable = value;
            }

        }

        private string _clientFunction = "alert('No function set!');";
        /// <summary>
        /// Function that can be called when the user click on a confimation button
        /// </summary>
        public string ClientFunction
        {
            get
            {
                if (_clientFunction.EndsWith(";"))
                    return _clientFunction;
                else
                    return _clientFunction + ";";
            }
            set
            {
                _clientFunction = value;
            }

        }
    }
}