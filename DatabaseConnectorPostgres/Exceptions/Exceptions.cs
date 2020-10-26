using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnectorPostgres.Exceptions
{

	public class MultipleUsersFoundException : Exception
	{
		public MultipleUsersFoundException() : base("Es wurden mehrere Nutzer mit der angegebenen E-Mail-Adresse gefunden.")
		{
		}
	}

	public class DbConnectionExcetion : Exception
	{
		public DbConnectionExcetion() : base("Connection Exception")
		{
		}
	}

	public class GetUpdateRowStringException : Exception
	{
		public GetUpdateRowStringException() : base("Update Row String Exception")
		{
		}
	}

	public class GetInsertRowStringException : Exception
	{
		public GetInsertRowStringException() : base("Insert Row String Exception")
		{
		}
	}

	public class NeedsUpdateException : Exception
	{
		public NeedsUpdateException() : base("Needs Update Exception")
		{
		}
	}

	public class GetDbFeatureAttributeException : Exception
	{
		public GetDbFeatureAttributeException() : base("Get DbFeatureAttribute Exception")
		{
		}
	}


	public class GetFeaturesException : Exception
	{
		public GetFeaturesException() : base("Get Features Exception")
		{
		}
	}

	public class CreateFeatureException : Exception
	{
		public CreateFeatureException() : base("Create Feature Exception")
		{
		}
	}

	public class InsertFeatureException : Exception
	{
		public InsertFeatureException() : base("Insert Feature Exception")
		{
		}
	}


	public class InsertFeaturesException : Exception
	{
		public InsertFeaturesException() : base("Insert Features Exception")
		{
		}
	}

	public class UpdateFeatureException : Exception
	{
		public UpdateFeatureException() : base("Update Feature Exception")
		{
		}
	}

	public class UpdateFeaturesException : Exception
	{
		public UpdateFeaturesException() : base("Update Features Exception")
		{
		}
	}

	public class DeleteFeaturesException : Exception
	{
		public DeleteFeaturesException() : base("Delete Features Exception")
		{
		}
	}

	public class DbFeatureClassAttributeException : Exception
	{
		public DbFeatureClassAttributeException() : base("DbFeatureClassAttributeException Exception")
		{
		}
	}

	public class ToNameArrayException : Exception
	{
		public ToNameArrayException() : base("ToNameArrayException Exception")
		{
		}
	}
	public class DbFeatureClassException : Exception
	{
		public DbFeatureClassException() : base("DbFeatureClassException Exception")
		{
		}
	}

	public class WrongPasswordException : Exception
	{
		public WrongPasswordException() : base("Falsches Passwort.")
		{
		}
	}
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException() : base("Der angegebene Benutzername ist unbekannt.")
		{
		}
	}

	public class GetAllUserException : Exception
	{
		public GetAllUserException() : base("GetAllUserException Exception")
		{
		}
	}
	

}
