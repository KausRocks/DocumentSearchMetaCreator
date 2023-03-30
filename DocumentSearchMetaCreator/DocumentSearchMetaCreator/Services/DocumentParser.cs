using DocumentSearchMetaCreator.Models;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing;

namespace DocumentSearchMetaCreator.Services
{
    public class DocumentParser
    {
        private string[] stopWordsAppend;
        private string datetimeFormatString = "dd-MKM-yyyy hh:mm:ss";
        private string[] supportedDocuments = { ".pdf", ".doc", ".docx", ".txt" };

        #region Constructors

        public DocumentParser()
        {
            stopWordsAppend = null;
        }

        public DocumentParser(string[] addonStopWords)
        {
            stopWordsAppend = addonStopWords;
        }

        public DocumentParser(string dateTimeFormat)
        {
            datetimeFormatString = dateTimeFormat;
        }

        public DocumentParser(string[] addonStopWords, string dateTimeFormat)
        {
            stopWordsAppend = addonStopWords;
            datetimeFormatString = dateTimeFormat;
        }

        #endregion

        #region Private Method

        private Dictionary<string, string> GetFileMetaInfo(FileInfo info)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("Name", info.Name);
            metadata.Add("FullName", info.FullName);
            metadata.Add("Extension", info.Extension);
            metadata.Add("DirectoryName", info.DirectoryName == null ? "" : info.DirectoryName);

            // File Exists ?
            bool exists = info.Exists;
            if (info.Exists)
            {
                metadata.Add("SizeBytes", info.Length.ToString());
                metadata.Add("IsReadonly", info.IsReadOnly.ToString());
                metadata.Add("CreationTime", info.CreationTime.ToString(datetimeFormatString));
                metadata.Add("LastAccessTime", info.LastAccessTime.ToString(datetimeFormatString));
                metadata.Add("LastWriteTime", info.LastWriteTime.ToString(datetimeFormatString));
            }

            return metadata;
        }


        private string[] FetchStopKeywords()
        {
            var direcory = Directory.GetCurrentDirectory();
            var stopWordsPath = System.IO.Path.Combine(direcory, "stopwords.txt");
            string[] stopwords = System.IO.File.ReadAllLines(stopWordsPath);

            if (stopWordsAppend != null)
            {
                stopwords = stopwords.Concat(stopWordsAppend).ToArray();
            }

            return stopwords.Distinct().ToArray();
        }

        #endregion

        #region Public Method

        public MetaDataModel ParseDocument(string documentPath)
        {
            string[] stopWords = FetchStopKeywords();
            Dictionary<string, string> metadata = GetFileMetaInfo(new FileInfo(documentPath));
            if (metadata == null)
                metadata = new Dictionary<string, string>();

            string extension = "";
            if (!metadata.TryGetValue("Extension", out extension))
            {
                extension = System.IO.Path.GetExtension(documentPath).ToLower();
            }
            else
            {
                extension = metadata["Extension"].ToLower();
            }

            if (!supportedDocuments.Contains(extension))
                throw new Exception("Document with extension " + extension + " is currently not supported");

            string s = extension;

            string fileContent = extension switch
            {
                ".pdf" => FetchPdfContent(documentPath),
                ".txt" => FetchTextContent(documentPath),
                ".doc" => FetchDocContent(documentPath, extension),
                ".docx" => FetchDocContent(documentPath, extension),
                _ => ""
            };

            var filteredWords = fileContent.Split(' ')
                    .Where(w => !stopWords.Contains(w.ToLower()))
                    .Select(w => w.Trim())
                    .Distinct()
                    .ToArray();

            string filteredText = string.Join(" ", filteredWords);

            return new MetaDataModel()
            {
                DocumentMetaData = metadata,
                FullContent = fileContent,
                SearchableContent = filteredText
            };
        }

        private string FetchPdfContent(string pdfAbsolutePath)
        {
            string fileContent = "";
            var codePages = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(codePages);
            PdfReader reader2 = new PdfReader((string)pdfAbsolutePath);

            iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader2);



            int numberofpages = pdfDoc.GetNumberOfPages();
            for (int page = 1; page <= numberofpages; page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(
                    Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                fileContent += currentText;
            }

            reader2.Close();

            return fileContent;
        }

        public string FetchTextContent(string txtAbsolutePath)
        {

            string fileContent = System.IO.File.ReadAllText(txtAbsolutePath);

            return fileContent;
        }

        public string FetchDocContent(string docAbsolutePath, string extension)
        {
            string fileContent = "";

            using (var doc = WordprocessingDocument.Open(docAbsolutePath, false))
            {
                //foreach (var el in doc.MainDocumentPart.Document.Body.Elements().OfType<Paragraph>())
                //{
                //    fileContent += el.InnerText;
                //}

                fileContent = doc.MainDocumentPart.Document.Body.InnerText;
            }


            return fileContent;
        }

        #endregion
    }
}
