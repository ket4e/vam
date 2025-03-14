using System;
using System.Collections;

namespace Mono.WebBrowser;

public class Exception : System.Exception
{
	internal enum ErrorCodes
	{
		Other,
		GluezillaInit,
		EngineNotSupported,
		ServiceManager,
		IOService,
		DirectoryService,
		PrefService,
		StreamNotOpen,
		Navigation,
		AccessibilityService,
		DocumentEncoderService
	}

	private ErrorCodes code;

	private static ArrayList messages;

	internal ErrorCodes ErrorCode => code;

	internal Exception(ErrorCodes code)
		: base(GetMessage(code, string.Empty))
	{
		this.code = code;
	}

	internal Exception(ErrorCodes code, string message)
		: base(GetMessage(code, message))
	{
		this.code = code;
	}

	internal Exception(ErrorCodes code, System.Exception innerException)
		: base(GetMessage(code, string.Empty), innerException)
	{
		this.code = code;
	}

	internal Exception(ErrorCodes code, string message, Exception innerException)
		: base(GetMessage(code, message), innerException)
	{
		this.code = code;
	}

	static Exception()
	{
		messages = new ArrayList();
		messages.Insert(0, string.Intern("A critical error occurred."));
		messages.Insert(1, string.Intern("An error occurred while initializing gluezilla. Please make sure you have libgluezilla installed."));
		messages.Insert(2, string.Intern("Browser engine not supported at this time: "));
		messages.Insert(3, string.Intern("Error obtaining a handle to the service manager."));
		messages.Insert(4, string.Intern("Error obtaining a handle to the io service."));
		messages.Insert(5, string.Intern("Error obtaining a handle to the directory service."));
		messages.Insert(6, string.Intern("Error obtaining a handle to the preferences service."));
		messages.Insert(7, string.Intern("Stream is not open for writing. Call OpenStream before appending."));
		messages.Insert(8, string.Intern("An error occurred while initializing the navigation object."));
		messages.Insert(9, string.Intern("Error obtaining a handle to the accessibility service."));
		messages.Insert(10, string.Intern("Error obtaining a handle to the document encoder service."));
	}

	private static string GetMessage(ErrorCodes code, string message)
	{
		string text = messages[(int)code] as string;
		return text + " " + message;
	}
}
