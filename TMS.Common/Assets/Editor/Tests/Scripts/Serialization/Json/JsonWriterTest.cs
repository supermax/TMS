using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NUnit.Framework;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Test.Serialization.Json
{
	// TODO: Add methods for check conversion of all written values
	[TestFixture]
	public sealed class JsonWriterTest
	{
		#region Tests

		//[Test]
		//public void JsonWriter_State_Disposing_Supported()
		//{
		//	var writer = new JsonWriter();
		//	var disposable = writer as IDisposable;
		//	Assert.IsNotNull(disposable);
		//}

		//[Test]
		//public void JsonWriter_Construction_TextWriter_Created()
		//{
		//	var writer = new JsonWriter();
		//	Assert.IsNotNull(writer.TextWriter);
		//}

		[Test]
		public void XmlSerializationTest()
		{
			using (var str = new System.IO.StringWriter())
			{
				var s = new XmlSerializer(typeof(XmlSerializationTest));
				s.Serialize(str, new XmlSerializationTest{ Path = new UriExt("http://www.diwip.com")});

				var res = str.ToString();
				Assert.IsNotNull(res);
			}
		}

		#endregion
	}

	[Serializable]
	public class UriExt : Uri
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UriExt"/> class.
		/// </summary>
		public UriExt()
			: this(null)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Uri"/> class with the specified URI.
		/// </summary>
		/// <param name="uriString">A URI. </param><exception cref="T:System.ArgumentNullException"><paramref name="uriString"/> is null. </exception><exception cref="T:System.UriFormatException"><paramref name="uriString"/> is empty.-or- The scheme specified in <paramref name="uriString"/> is not correctly formed. See <see cref="M:System.Uri.CheckSchemeName(System.String)"/>.-or- <paramref name="uriString"/> contains too many slashes.-or- The password specified in <paramref name="uriString"/> is not valid.-or- The host name specified in <paramref name="uriString"/> is not valid.-or- The file name specified in <paramref name="uriString"/> is not valid. -or- The user name specified in <paramref name="uriString"/> is not valid.-or- The host or authority name specified in <paramref name="uriString"/> cannot be terminated by backslashes.-or- The port number specified in <paramref name="uriString"/> is not valid or cannot be parsed.-or- The length of <paramref name="uriString"/> exceeds 65519 characters.-or- The length of the scheme specified in <paramref name="uriString"/> exceeds 1023 characters.-or- There is an invalid character sequence in <paramref name="uriString"/>.-or- The MS-DOS path specified in <paramref name="uriString"/> must start with c:\\.</exception>
		public UriExt(string uriString) : base(uriString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Uri"/> class with the specified URI. This constructor allows you to specify if the URI string is a relative URI, absolute URI, or is indeterminate.
		/// </summary>
		/// <param name="uriString">A string that identifies the resource to be represented by the <see cref="T:System.Uri"/> instance.</param><param name="uriKind">Specifies whether the URI string is a relative URI, absolute URI, or is indeterminate.</param><exception cref="T:System.ArgumentException"><paramref name="uriKind"/> is invalid. </exception><exception cref="T:System.ArgumentNullException"><paramref name="uriString"/> is null. </exception><exception cref="T:System.UriFormatException"><paramref name="uriString"/> contains a relative URI and <paramref name="uriKind"/> is <see cref="F:System.UriKind.Absolute"/>.or<paramref name="uriString"/> contains an absolute URI and <paramref name="uriKind"/> is <see cref="F:System.UriKind.Relative"/>.or<paramref name="uriString"/> is empty.-or- The scheme specified in <paramref name="uriString"/> is not correctly formed. See <see cref="M:System.Uri.CheckSchemeName(System.String)"/>.-or- <paramref name="uriString"/> contains too many slashes.-or- The password specified in <paramref name="uriString"/> is not valid.-or- The host name specified in <paramref name="uriString"/> is not valid.-or- The file name specified in <paramref name="uriString"/> is not valid. -or- The user name specified in <paramref name="uriString"/> is not valid.-or- The host or authority name specified in <paramref name="uriString"/> cannot be terminated by backslashes.-or- The port number specified in <paramref name="uriString"/> is not valid or cannot be parsed.-or- The length of <paramref name="uriString"/> exceeds 65519 characters.-or- The length of the scheme specified in <paramref name="uriString"/> exceeds 1023 characters.-or- There is an invalid character sequence in <paramref name="uriString"/>.-or- The MS-DOS path specified in <paramref name="uriString"/> must start with c:\\.</exception>
		public UriExt(string uriString, UriKind uriKind) : base(uriString, uriKind)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Uri"/> class based on the specified base URI and relative URI string.
		/// </summary>
		/// <param name="baseUri">The base URI. </param><param name="relativeUri">The relative URI to add to the base URI. </param><exception cref="T:System.ArgumentNullException"><paramref name="baseUri"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="baseUri"/> is not an absolute <see cref="T:System.Uri"/> instance. </exception><exception cref="T:System.UriFormatException">The URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is empty or contains only spaces.-or- The scheme specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> contains too many slashes.-or- The password specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The host name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The file name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid. -or- The user name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The host or authority name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> cannot be terminated by backslashes.-or- The port number specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid or cannot be parsed.-or- The length of the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> exceeds 65519 characters.-or- The length of the scheme specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> exceeds 1023 characters.-or- There is an invalid character sequence in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/>.-or- The MS-DOS path specified in <paramref name="uriString"/> must start with c:\\.</exception>
		public UriExt(Uri baseUri, string relativeUri) : base(baseUri, relativeUri)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Uri"/> class based on the combination of a specified base <see cref="T:System.Uri"/> instance and a relative <see cref="T:System.Uri"/> instance.
		/// </summary>
		/// <param name="baseUri">An absolute <see cref="T:System.Uri"/> that is the base for the new <see cref="T:System.Uri"/> instance. </param><param name="relativeUri">A relative <see cref="T:System.Uri"/> instance that is combined with <paramref name="baseUri"/>. </param><exception cref="T:System.ArgumentException"><paramref name="baseUri"/> is not an absolute <see cref="T:System.Uri"/> instance. </exception><exception cref="T:System.ArgumentNullException"><paramref name="baseUri"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="baseUri"/> is not an absolute <see cref="T:System.Uri"/> instance. </exception><exception cref="T:System.UriFormatException">The URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is empty or contains only spaces.-or- The scheme specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> contains too many slashes.-or- The password specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The host name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The file name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid. -or- The user name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid.-or- The host or authority name specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> cannot be terminated by backslashes.-or- The port number specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> is not valid or cannot be parsed.-or- The length of the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> exceeds 65519 characters.-or- The length of the scheme specified in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/> exceeds 1023 characters.-or- There is an invalid character sequence in the URI formed by combining <paramref name="baseUri"/> and <paramref name="relativeUri"/>.-or- The MS-DOS path specified in <paramref name="uriString"/> must start with c:\\.</exception>
		public UriExt(Uri baseUri, Uri relativeUri) : base(baseUri, relativeUri)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Uri"/> class from the specified instances of the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> and <see cref="T:System.Runtime.Serialization.StreamingContext"/> classes.
		/// </summary>
		/// <param name="serializationInfo">An instance of the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> class containing the information required to serialize the new <see cref="T:System.Uri"/> instance. </param><param name="streamingContext">An instance of the <see cref="T:System.Runtime.Serialization.StreamingContext"/> class containing the source of the serialized stream associated with the new <see cref="T:System.Uri"/> instance. </param><exception cref="T:System.ArgumentNullException">The <paramref name="serializationInfo"/> parameter contains a null URI. </exception><exception cref="T:System.UriFormatException">The <paramref name="serializationInfo"/> parameter contains a URI that is empty.-or- The scheme specified is not correctly formed. See <see cref="M:System.Uri.CheckSchemeName(System.String)"/>.-or- The URI contains too many slashes.-or- The password specified in the URI is not valid.-or- The host name specified in URI is not valid.-or- The file name specified in the URI is not valid. -or- The user name specified in the URI is not valid.-or- The host or authority name specified in the URI cannot be terminated by backslashes.-or- The port number specified in the URI is not valid or cannot be parsed.-or- The length of URI exceeds 65519 characters.-or- The length of the scheme specified in the URI exceeds 1023 characters.-or- There is an invalid character sequence in the URI.-or- The MS-DOS path specified in the URI must start with c:\\.</exception>
		protected UriExt(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}
	}

	[XmlRoot(ElementName = "test")]
	public class XmlSerializationTest
	{
		[XmlElement(ElementName = "path")]
		public UriExt Path { get; set; }
	}
}
