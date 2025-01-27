# DXT1 Decompressor

DXT1 Decompressor is a simple C# application designed to decompress DXT1 compressed textures from LRF (Lightweight Resource File) archives and convert them into standard BMP bitmap images. This tool is especially useful for developers and artists working with game assets or other applications that utilize DXT1 compression.

## 📦 Features
- [x] DXT1 Decompression: Efficiently decompresses DXT1 compressed textures.
- [x] LRF Support: Reads and extracts texture data from LRF files.
- [x] Command-Line Interface: Accepts file paths as command-line arguments for streamlined workflows.
- [x] Interactive Mode: Prompts users for file paths when none are provided via the terminal.
- [x] Unicode & Emojis: Enhanced console output with Unicode and emojis for a user-friendly experience.
- [x] Colored Messages: Utilizes colored console messages to indicate status, errors, and successes.
- [x] Automatic Naming: Saves the output bitmap with the same name as the input file for consistency.
- [ ] Converting and decompressing TGA files.
- [ ] GUI interface

##🚀 Getting Started

### 🔧 Prerequisites
.NET 8.0 Framework: Ensure you have the .NET Framework installed. You can download it from here.
System.Drawing: The application uses System.Drawing for bitmap manipulation. Ensure your environment supports it.

###🛠️ Building the Project
Clone the Repository
```bash
git clone https://github.com/yourusername/DXT1Decompressor.git
```
Navigate to the Project Directory
```bash
cd DXT1Decompressor
```
Build the Project
```bash
dotnet build
```
🏃 Running the Application
📁 Using Command-Line Arguments
You can provide the path to the LRF file directly when running the application.

```bash
dotnet run -- "path/to/your/file.lrf"
```
Example:

```bash
dotnet run -- "C:\Textures\uv_grid.lrf"
```
Upon successful execution, the decompressed bitmap will be saved in the same directory as the input file with the .bmp extension.

🖥️ Interactive Mode
If you run the application without any arguments, it will prompt you to enter the path to the LRF file.

```bash
dotnet run
```
Sample Interaction:

![alt text](https://github.com/Dimiqhz/CS-DTX1-Decompressor/tree/main/screenshots/example.png)

The resulting bitmap will have the same name as the input file but with a .bmp extension.

##📝 Notes
Ensure that the LRF files you intend to decompress contain textures. The application currently supports textures compressed with DXT1.
If the application encounters issues with the file (e.g., invalid format, unsupported compression), it will display appropriate error messages with colored indicators.

## 🧩 Contributing
Contributions are welcome! Please fork the repository and submit a pull request for any enhancements or bug fixes.

- Fork the repository.
- Create your feature branch: `git checkout -b feature/YourFeature`
- Commit your changes: `git commit -m "Add some feature"`
- Push to the branch: `git push origin feature/YourFeature`
- Open a pull request.
- 
## 📜 License
This project is licensed under the GNU License. See the LICENSE file for details.

## 📫 Contact
For any questions or suggestions, feel free to open an issue or contact contact@dimiqhz.pro.

