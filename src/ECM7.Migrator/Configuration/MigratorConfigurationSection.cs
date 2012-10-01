﻿namespace ECM7.Migrator.Configuration
{
	using System.Configuration;

	/// <summary>
	/// Настройки мигратора
	/// </summary>
	public class MigratorConfigurationSection : ConfigurationSection, IMigratorConfiguration
	{
		/// <summary>
		/// Provider
		/// </summary>
		[ConfigurationProperty("provider", IsRequired = true)]
		public string Provider
		{
			get { return (string)base["provider"]; }
		}

		/// <summary>
		/// Строка подключения
		/// </summary>
		[ConfigurationProperty("connectionString")]
		public string ConnectionString
		{
			get { return (string)base["connectionString"]; }
		}

		/// <summary>
		/// Название строки подключения
		/// </summary>
		[ConfigurationProperty("connectionStringName")]
		public string ConnectionStringName
		{
			get { return (string)base["connectionStringName"]; }
		}

		/// <summary>
		/// Сборка с миграциями
		/// </summary>
		[ConfigurationProperty("assembly")]
		public string Assembly
		{
			get { return (string)base["assembly"]; }
		}

		/// <summary>
		/// Путь к файлу с миграциями
		/// </summary>
		[ConfigurationProperty("assemblyFile")]
		public string AssemblyFile
		{
			get { return (string)base["assemblyFile"]; }
		}

		/// <summary>
		/// Максимальное время выполнения команды
		/// </summary>
		[ConfigurationProperty("commandTimeout")]
		public int CommandTimeout
		{
			get
			{
				var value = (base["commandTimeout"] ?? string.Empty) as string;
				int result;

				if (int.TryParse(value, out result))
				{
					return result;
				}

				return default(int);
			}
		}
	}
}
