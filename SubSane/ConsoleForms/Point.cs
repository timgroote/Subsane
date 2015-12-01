using System;
using System.Xml.Serialization;

namespace SubSane.ConsoleForms
{
    /// <summary>
    /// An object to use to specify the X and Y coordinates of an object on the console
    /// screen.  A <see cref="Point">Point</see> object already exists in the framework, 
    /// but it's hidden in System.Windows.Forms.Drawing, which I didn't want to include 
    /// here. 
    /// </summary>
    public class Point : IXmlSerializable
    {
        #region Private Member Variables

        private int _x = 0;
        private int _y = 0;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of a <see cref="Point">Point</see> object.
        /// </summary>
        public Point()
        {
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Point">Point</see> object when
        /// the values for the coordinates are known at construction time.
        /// </summary>
        /// <param name="x">A 32-bit integer storing the X position of the console object 
        /// on the screen.</param>
        /// <param name="y">A 32-bit integer storing the Y position of the console object 
        /// on the screen.</param>
        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The X position of the object on the screen.
        /// </summary>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// The Y position of the object on the screen.
        /// </summary>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Gets the string representation of the Point object.
        /// </summary>
        /// <returns>A representation of the state of the Point object.</returns>
        /// <remarks>This method is useful during debugging to get an easy-to-read representation of the object.</remarks>
        public override string ToString()
        {
            return "X: " + _x + " Y: " + _y;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="Point">Point</see>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Location");
            writer.WriteAttributeString(string.Empty, "X", string.Empty, X.ToString());
            writer.WriteAttributeString(string.Empty, "Y", string.Empty, Y.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Method that returns schema information.  Not yet implemented.
        /// </summary>
        /// <returns></returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads Xml when the <see cref="Point">Point</see> is to be deserialized 
        /// from a stream.</summary>
        /// <param name="reader">The stream from which the object will be deserialized.</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            string x = reader.GetAttribute("X");
            string y = reader.GetAttribute("Y");

            if (x.Length > 0)
                _x = int.Parse(x);

            if (y.Length > 0)
                _y = int.Parse(y);
        }

        #endregion
    }
}