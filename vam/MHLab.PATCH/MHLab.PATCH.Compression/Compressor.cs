using MHLab.PATCH.Compression.TAR;
using MHLab.PATCH.Compression.ZIP;

namespace MHLab.PATCH.Compression;

internal class Compressor
{
	public static void Compress(string folderToCompress, string outputFile, CompressionType type, string password)
	{
		switch (type)
		{
		case CompressionType.ZIP:
			ZIPCompressor.ZipFolder(outputFile, password, folderToCompress);
			break;
		case CompressionType.TAR:
			TARGZCompressor.ArchiveFolder(outputFile, folderToCompress, compress: false);
			break;
		case CompressionType.TARGZ:
			TARGZCompressor.ArchiveFolder(outputFile, folderToCompress, compress: true);
			break;
		}
	}

	public static void Decompress(string folderWhereDecompress, string inputFile, CompressionType type, string password)
	{
		switch (type)
		{
		case CompressionType.ZIP:
			ZIPCompressor.ExtractZipFile(inputFile, password, folderWhereDecompress);
			break;
		case CompressionType.TAR:
			TARGZCompressor.ExtractTAR(inputFile, folderWhereDecompress);
			break;
		case CompressionType.TARGZ:
			TARGZCompressor.ExtractTGZ(inputFile, folderWhereDecompress);
			break;
		}
	}
}
