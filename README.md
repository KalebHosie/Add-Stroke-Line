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

pdf-stroke-adder --file example.pdf --rotation 180 --debug False
