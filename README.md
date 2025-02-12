# Add Stroke

PDF Stroke Adder

This is a .NET command-line tool that modifies PDF files by adding a stroke along the page boundary at a specified rotation angle. It leverages iText7 to process PDFs and supports spot colors for precise printing needs.

Features:

✔️ Add a stroke to a PDF at 0°, 90°, 180°, or 270° rotation

✔️ Supports spot colors (CMYK) for professional printing workflows

✔️ Debug mode (--debug) saves output in a separate "Stroke Debug" folder

✔️ Overwrites the original PDF by default unless debug mode is enabled

Usage:
pdf-stroke-adder --file input.pdf --rotation 90 --debug
Arguments:

-f, --file → Path to the PDF file
-r, --rotation → Rotation angle (0, 90, 180, or 270)
-d, --debug → Enable debug mode to save output separately
Example Commands:
👉 Add a stroke at 90° rotation:

pdf-stroke-adder -f example.pdf -r 90
👉 Enable debug mode (preserve the original file):

pdf-stroke-adder --file example.pdf --rotation 180 --debug
Dependencies:
.NET 6+
iText7 PDF library
Installation & Build Instructions:
Clone the repository and build using .NET CLI:

git clone https://github.com/your-repo/pdf-stroke-adder.git
cd pdf-stroke-adder
dotnet build
dotnet run -- --file input.pdf --rotation 0
Error Handling & Logging
If an invalid PDF path is provided, the tool throws an error message
If an unsupported rotation angle is used, it alerts the user
Errors during PDF modification are caught and displayed
This tool is ideal for print professionals, graphic designers, and automated PDF workflows requiring precise strokes. 🚀
