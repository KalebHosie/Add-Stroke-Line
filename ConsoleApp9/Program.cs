using iTextSharp.text;
using iTextSharp.text.pdf;
using System.CommandLine;

class Program
{
    // Removed FixedKey and associated metadata checks.
    // Kept SpotColor for color usage only.
    private const string DebugFolderName = "Stroke Debug";
    private const string SpotColor = "tagkisscut";

    public static int Main(string[] args)
    {
        var fileOption = new Option<string>(
            new[] { "--file", "-f" },
            description: "Path to the PDF file to modify",
            getDefaultValue: () => string.Empty);

        var rotationOption = new Option<int>(
            new[] { "--rotation", "-r" },
            description: "Rotation angle: 0, 90, 180, or 270",
            getDefaultValue: () => 0);

        var debugOption = new Option<bool>(
            new[] { "--debug", "-d" },
            description: "Debug true or false",
            getDefaultValue: () => false);

        RootCommand rootCommand = new()
        {
            fileOption,
            rotationOption,
            debugOption
        };

        rootCommand.Description = "Add a stroke to a PDF at a specified rotation";

        rootCommand.SetHandler((string file, int rotation, bool debug) =>
        {
            Handler(file, rotation, debug);
        }, fileOption, rotationOption, debugOption);

        return rootCommand.Invoke(args);
    }

    static void Handler(string file, int rotation, bool debug)
    {
        if (string.IsNullOrEmpty(file) || !File.Exists(file))
        {
            Console.WriteLine("Error: Invalid PDF file path.");
            return;
        }

        if (rotation != 0 && rotation != 90 && rotation != 180 && rotation != 270)
        {
            Console.WriteLine("Error: Invalid rotation value. Accepted values are 0, 90, 180, or 270.");
            return;
        }
        Console.WriteLine($"Adding stroke to '{file}' with rotation {rotation}... debug: {debug}");
        AddStrokeToPdf(file, rotation, debug);
    }

    static string AddDirectoryToFilePath(string filePath, string newDirectory)
    {
        string originalDirectory = Path.GetDirectoryName(filePath);
        string updatedDirectory = Path.Combine(originalDirectory, newDirectory);

        Directory.CreateDirectory(updatedDirectory);

        string updatedPath = Path.Combine(updatedDirectory, Path.GetFileName(filePath));
        return updatedPath;
    }

    //   dotnet AddStrokeLine.dll --file "D:\baka.pdf" --rotation 90 --debug true
    static void AddStrokeToPdf(string inputPdf, int rotation, bool debug)
    {
        // Create a temporary file for writing
        string outputFile = debug
            ? AddDirectoryToFilePath(inputPdf, DebugFolderName)
            : Path.Combine(Path.GetDirectoryName(inputPdf) ?? string.Empty, Guid.NewGuid() + ".pdf");

        try
        {
            using (var reader = new PdfReader(inputPdf))
            {
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var stamper = new PdfStamper(reader, outputStream))
                {
                    BaseColor baseColor = new CMYKColor(0f, 1f, 0f, 0f);
                    PdfSpotColor spotColor = new PdfSpotColor(SpotColor, baseColor);
                    PdfGState gState = new PdfGState
                    {
                        FillOpacity = 1.0f,
                        StrokeOpacity = 1.0f
                    };

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfContentByte cb = stamper.GetOverContent(i);
                        cb.SetGState(gState);
                        cb.SetColorStroke(new SpotColor(spotColor, 1.0f));

                        var pageSize = reader.GetPageSize(i);
                        float x1, y1, x2, y2;

                        switch (rotation)
                        {
                            case 0:
                                x1 = pageSize.Left;
                                y1 = pageSize.Bottom;
                                x2 = pageSize.Right;
                                y2 = pageSize.Bottom;
                                break;
                            case 90:
                                x1 = pageSize.Right;
                                y1 = pageSize.Bottom;
                                x2 = pageSize.Right;
                                y2 = pageSize.Top;
                                break;
                            case 180:
                                x1 = pageSize.Left;
                                y1 = pageSize.Top;
                                x2 = pageSize.Right;
                                y2 = pageSize.Top;
                                break;
                            case 270:
                                x1 = pageSize.Left;
                                y1 = pageSize.Bottom;
                                x2 = pageSize.Left;
                                y2 = pageSize.Top;
                                break;
                            default:
                                throw new InvalidOperationException("Invalid rotation value.");
                        }

                        DrawStroke(cb, x1, y1, x2, y2);
                    }
                }
            }

            if (!debug)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Delete(inputPdf);
                File.Move(outputFile, inputPdf);
            }

            Console.WriteLine($"StrokeLine added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            if (File.Exists(outputFile) && !debug)
            {
                File.Delete(outputFile);
            }
            throw;
        }
    }

    static void DrawStroke(PdfContentByte cb, float x1, float y1, float x2, float y2)
    {
        cb.SetLineWidth(.5f);
        cb.MoveTo(x1, y1);
        cb.LineTo(x2, y2);
        cb.Stroke();
    }
}
