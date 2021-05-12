using iText.Forms;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemovePDFSignature
{
    class Program
    {
        /// <summary>
        /// Removes digital signature from PDF files using iText7.
        /// </summary>
        /// <param name="args">Name of input pdf files. Wildchars (*, ?) can be used too.</param>
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No input file was given in parameters.");
                return;
            }

            List<string> inputlist = new List<string>();
            foreach (var arg in args)
            {
                if (arg.Contains("*") || arg.Contains("?"))
                {
                    string dir = ".";
                    if (arg.Contains(Path.DirectorySeparatorChar))
                    {
                        dir = Path.GetDirectoryName(arg);
                    }
                    inputlist.AddRange(Directory.GetFiles(dir, Path.GetFileName(arg), SearchOption.TopDirectoryOnly));
                }
                else
                {
                    inputlist.Add(arg);
                }
            }

            foreach (var inputfile in inputlist)
            {
                Console.WriteLine($"Processing {inputfile} ...");
                string dir = ".";
                if (inputfile.Contains(Path.DirectorySeparatorChar))
                {
                    dir = Path.GetDirectoryName(inputfile);
                }
                string outfilename = Path.Combine(dir, Path.GetFileNameWithoutExtension(inputfile) + "_unsigned" + Path.GetExtension(inputfile));


                try
                {
                    //https://kb.itextpdf.com/home/it7kb/examples/remove-digital-signatures
                    using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputfile), new PdfWriter(outfilename)))
                    {
                        PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                        // If no fields have been explicitly included, then all fields are flattened.
                        // Otherwise only the included fields are flattened.
                        form.FlattenFields();

                        pdfDoc.Close();
                    }
                    Console.WriteLine($"Created {outfilename}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to process {inputfile}, because: {ex.Message}");
                    Debug.WriteLine(ex);
                }

            }
        }
    }
}
