using OpencorporaConverter.Classes;
using System.Xml.Serialization;

string filePath = "";
bool isCorrectFilePath = false;

while (!isCorrectFilePath)
{
    Console.Write($"Enter path to OpenCorpora file: ");
    filePath = Console.ReadLine()!;

    if (!File.Exists(filePath))
    {
        Console.WriteLine("File doesn't exist. Try again\n");
        continue;
    }

    if (!Path.HasExtension(filePath) || !Path.GetExtension(filePath).Equals(".xml"))
    {
        Console.WriteLine("File must have xml extension. Try again\n");
        continue;
    }

    isCorrectFilePath = true;
}

try
{
    if (!File.Exists(filePath))
        throw new Exception("File doesn't exist");

    if (!Path.HasExtension(filePath) ||
        !Path.GetExtension(filePath).Equals(".xml"))
        throw new Exception("File must have the extension .xml");

    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Opencorpora));
    using (FileStream fs = new FileStream(Path.GetFullPath(filePath), FileMode.Open))
    {
        Opencorpora? opencorpora = xmlSerializer.Deserialize(fs) as Opencorpora;

        if (opencorpora != null)
        {
            Console.WriteLine($"\nOpencorpora v.{opencorpora.Version}.{opencorpora.Revision}");
            Converter converter = new Converter(opencorpora, filePath);
            converter.Run();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("\n" +
        $"Exception.Source: {ex.Source}\n" +
        $"Exception.TargetSite: {ex.TargetSite}\n" +
        $"Exception.Message: {ex.Message}\n");
}
