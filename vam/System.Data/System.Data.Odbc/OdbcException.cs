using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;

namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcException : DbException
{
	private OdbcErrorCollection odbcErrors;

	public OdbcErrorCollection Errors => odbcErrors;

	public override string Source => odbcErrors[0].Source;

	internal OdbcException(OdbcErrorCollection errors)
		: base(CreateMessage(errors))
	{
		odbcErrors = errors;
	}

	private OdbcException(SerializationInfo si, StreamingContext sc)
		: base(si, sc)
	{
		odbcErrors = new OdbcErrorCollection();
		odbcErrors = (OdbcErrorCollection)si.GetValue("odbcErrors", typeof(OdbcErrorCollection));
	}

	public override void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		if (si == null)
		{
			throw new ArgumentNullException("si");
		}
		si.AddValue("odbcErrors", odbcErrors, typeof(OdbcErrorCollection));
		base.GetObjectData(si, context);
	}

	private static string CreateMessage(OdbcErrorCollection errors)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < errors.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(Environment.NewLine);
			}
			OdbcError odbcError = errors[i];
			stringBuilder.AppendFormat("ERROR [{0}] {1}", odbcError.SQLState, odbcError.Message);
		}
		return stringBuilder.ToString();
	}
}
