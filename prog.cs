using System.Text;

List<string> files = Directory.GetFiles(Environment.CurrentDirectory, "*.txt").ToList();

files.ForEach(input =>
{
    string output = input[0..input.IndexOf(".txt")] + ".csv";
    string delimiter = "|";

    if (!File.Exists(input))
    {
        Console.WriteLine("Arquivo inválido");
        return;
    }

    if (File.Exists(output))
        File.Delete(output);

    StreamReader reader = new(input, Encoding.UTF8);
    StreamWriter writer = new(output, true, Encoding.UTF8);

    string content;

    int counter = 0;

    bool isHeader = true;

    while (reader.Peek() > 0)
    {
        content = reader.ReadLine()!;

        string[] splt = content.Split("  ", StringSplitOptions.RemoveEmptyEntries);

        splt = splt.Where(item => !String.IsNullOrWhiteSpace(item) && !String.IsNullOrEmpty(item)).ToArray();

        string txt = "";
        int len = splt.Length;
        int dateColQnt = 0;


        for (int i = 0; i < len; i++)
        {
            splt[i] = splt[i].Trim() + delimiter;

            if (!isHeader)
                dateColQnt += DateOnly.TryParse(splt[i].Replace(delimiter, ""), out _) || splt[i].Replace(delimiter, "").Length == 8 ? 1 : 0;

        }

        if (counter == 0 && !isHeader)
            splt = RemoveDataField(dateColQnt, ref len, splt, delimiter);

        for (int i = 0; i < len; i++)
            txt += splt[i];

        if (counter < 2)
        {
            writer.Write(txt);
            counter++;
        }
        else
        {
            txt = txt[0..(txt.Length - 1)];
            writer.WriteLine(txt);
            counter = 0;
            isHeader = false;
        }

        writer.Flush();
    }
});

static string[] RemoveDataField(int dateColQnt, ref int len, string[] splt, string delimiter)
{
    if (dateColQnt == 0)
    {
        for (int i = 0; i < 3; i++)
        {
            splt = splt.Append(delimiter).ToArray();
        }
        len += 3;
    }

    if (dateColQnt == 1)
    {
        splt = splt.Append(splt[3]).ToArray();
        splt[3] = delimiter;
        splt = splt.Append(delimiter).ToArray();
        len += 2;
    }

    if (dateColQnt == 2)
    {
        splt = splt.Append(delimiter).ToArray();
        len += 1;
    }

    return splt;
}