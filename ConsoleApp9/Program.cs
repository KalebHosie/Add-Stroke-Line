using System;
using System.CommandLine;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

class Program
{
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
            description: "Enable debug mode (saves output in 'Stroke Debug' folder)",
            getDefaultValue: () => false);

        RootCommand rootCommand = new()
        {
            fileOption,
            rotationOption,
            debugOption
        };

        rootCommand.Description = "Add a stroke to a PDF at a specified rotation.";

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

        Console.WriteLine($"Adding stroke to '{file}' with rotation {rotation}... Debug mode: {debug}");
        AddStrokeToPdf(file, rotation, debug);
    }

    static void AddStrokeToPdf(string inputPdf, int rotation, bool debug)
    {
        string debugFolderPath = Path.Combine(Path.GetDirectoryName(inputPdf) ?? string.Empty, DebugFolderName);

        // Ensure debug directory exists
        if (debug && !Directory.Exists(debugFolderPath))
        {
            Directory.CreateDirectory(debugFolderPath);
        }

        string outputFile = debug
            ? Path.Combine(debugFolderPath, Path.GetFileName(inputPdf))
            : Path.Combine(Path.GetDirectoryName(inputPdf) ?? string.Empty, Guid.NewGuid() + ".pdf");

        try
        {
            using (var reader = new PdfReader(inputPdf))
            using (var writer = new PdfWriter(outputFile))
            using (var pdfDoc = new PdfDocument(reader, writer))
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var pageSize = page.GetPageSize();
                    var canvas = new PdfCanvas(page);
                    var gState = new PdfExtGState().SetFillOpacity(1.0f).SetStrokeOpacity(1.0f);
                    canvas.SetExtGState(gState);

                    PdfSpecialCs.Separation spotColor = new PdfSpecialCs.Separation(
                        SpotColor,
                        new DeviceCmyk(0.0f, 1.0f, 0.0f, 0.0f).GetColorSpace(),
                        new PdfType2Function(
                            new double[] { 0, 1 },
                            null,
                            new double[] { 0, 0, 0, 0 },
                            new double[] { 0.0f, 1.0f, 0.0f, 0.0f },
                            1));

                    canvas.SetStrokeColor(new Separation(spotColor, 1.0f));

                    float x1, y1, x2, y2;
                    switch (rotation)
                    {
                        case 0:
                            x1 = pageSize.GetLeft();
                            y1 = pageSize.GetBottom();
                            x2 = pageSize.GetRight();
                            y2 = pageSize.GetBottom();
                            break;
                        case 90:
                            x1 = pageSize.GetRight();
                            y1 = pageSize.GetBottom();
                            x2 = pageSize.GetRight();
                            y2 = pageSize.GetTop();
                            break;
                        case 180:
                            x1 = pageSize.GetLeft();
                            y1 = pageSize.GetTop();
                            x2 = pageSize.GetRight();
                            y2 = pageSize.GetTop();
                            break;
                        case 270:
                            x1 = pageSize.GetLeft();
                            y1 = pageSize.GetBottom();
                            x2 = pageSize.GetLeft();
                            y2 = pageSize.GetTop();
                            break;
                        default:
                            throw new InvalidOperationException("Invalid rotation value.");
                    }

                    DrawStroke(canvas, x1, y1, x2, y2);
                }
            }

            if (!debug)
            {
                File.Delete(inputPdf);
                File.Move(outputFile, inputPdf);
            }

            Console.WriteLine($"Stroke added successfully. Output file: {outputFile}");
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

    static void DrawStroke(PdfCanvas canvas, float x1, float y1, float x2, float y2)
    {
        canvas.SetLineWidth(0.5f);
        canvas.MoveTo(x1, y1);
        canvas.LineTo(x2, y2);
        canvas.Stroke();
    }
}
