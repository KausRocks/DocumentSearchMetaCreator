// See https://aka.ms/new-console-template for more information
using DocumentSearchMetaCreator.Models;
using DocumentSearchMetaCreator.Services;

Console.WriteLine("Hello, World!");

var direcory = Directory.GetCurrentDirectory();
string pdfAbsolutePath = System.IO.Path.Combine(direcory, "World_Wide_Corp_lorem.pdf");

string txtAbsolutePath = System.IO.Path.Combine(direcory, "story.txt");
string docAbsolutePath = System.IO.Path.Combine(direcory, "World_Wide_Corp_salary.docx");
DocumentParser parsor = new DocumentParser();
MetaDataModel pdfResponseModel = parsor.ParseDocument(pdfAbsolutePath);
MetaDataModel txtResponseModel = parsor.ParseDocument(txtAbsolutePath);
MetaDataModel docResponseModel = parsor.ParseDocument(docAbsolutePath);

//Console.Write(pdfResponseModel.FullContent);

//Console.WriteLine();
//Console.WriteLine();

//Console.Write(pdfResponseModel.SearchableContent);

//Console.ReadLine();

//Console.WriteLine();
//Console.WriteLine();
//Console.Write(txtResponseModel.FullContent);
//Console.WriteLine();
//Console.WriteLine();
//Console.Write(txtResponseModel.SearchableContent);


//Console.ReadLine();


//Console.WriteLine();
//Console.WriteLine();
//Console.Write(docResponseModel.FullContent);
//Console.WriteLine();
//Console.WriteLine();
//Console.Write(docResponseModel.SearchableContent);


Console.ReadLine();