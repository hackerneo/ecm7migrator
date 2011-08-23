namespace ECM7.Migrator.Providers.Tests
{
	using System;
	using System.Data;

	using ECM7.Migrator.Framework;

	using NUnit.Framework;

	using ForeignKeyConstraint = ECM7.Migrator.Framework.ForeignKeyConstraint;

	/// <summary>
	/// Base class for Provider tests for all tests including constraint oriented tests.
	/// </summary>
	public abstract class TransformationProviderConstraintBase<TProvider> : TransformationProviderBase<TProvider>
		where TProvider : ITransformationProvider
	{

		public void AddForeignKey()
		{
			AddTableWithPrimaryKey();
			provider.AddForeignKey("FK_Test_TestTwo", "TestTwo", "TestId", "Test", "Id");
		}

		public void AddForeignKeyWithCommonAction()
		{
			AddTableWithPrimaryKey();
			provider.AddForeignKey("FK_Test_TestTwo", "TestTwo", "TestId", "Test", "Id", ForeignKeyConstraint.Cascade);
		}

		public void AddForeignKeyOnDeleteCascadeOnUpdateCascade()
		{
			AddTableWithPrimaryKey();
			provider.AddForeignKey("FK_Test_TestTwo", "TestTwo", new[] { "TestId" }, "Test", new[] { "Id" },
								   ForeignKeyConstraint.Cascade, ForeignKeyConstraint.NoAction);
		}

		public void AddUniqueConstraint()
		{
			provider.AddUniqueConstraint("UN_Test_TestTwo", "TestTwo", "TestId");
		}

		public void AddMultipleUniqueConstraint()
		{
			provider.AddUniqueConstraint("UN_Test_TestTwo", "TestTwo", "Id", "TestId");
		}

		public void AddCheckConstraint()
		{
			provider.AddCheckConstraint("CK_TestTwo_TestId", "TestTwo", provider.QuoteName("TestId") + " > 5");
		}

		[Test]
		public void CanAddPrimaryKey()
		{
			AddTableWithoutPrimaryKey();
			provider.AddPrimaryKey("PK_Test", "Test", "Id");
			Assert.IsTrue(provider.ConstraintExists("Test", "PK_Test"));
		}

		#region index

		[Test]
		public void CanAddIndex()
		{
			AddTableWithPrimaryKey();
			provider.AddIndex("ix_moo", false, "Test", new[] { "Name" });
			Assert.IsTrue(provider.IndexExists("ix_moo", "Test"));

			provider.RemoveIndex("ix_moo", "Test");
			Assert.IsFalse(provider.IndexExists("ix_moo", "Test"));
		}

		[Test]
		public void CanAddUniqueIndex()
		{
			AddTableWithPrimaryKey();
			provider.AddIndex("ix_moo", true, "Test", new[] { "Name" });
			Assert.IsTrue(provider.IndexExists("ix_moo", "Test"));

			provider.RemoveIndex("ix_moo", "Test");
			Assert.IsFalse(provider.IndexExists("ix_moo", "Test"));

		}

		[Test]
		public void CanAddComplexIndex()
		{
			AddTableWithPrimaryKey();
			provider.AddIndex("ix_moo", false, "Test", new[] { "Id", "Title" });
			Assert.IsTrue(provider.IndexExists("ix_moo", "Test"));

			provider.RemoveIndex("ix_moo", "Test");
			Assert.IsFalse(provider.IndexExists("ix_moo", "Test"));

		}

		#endregion

		[Test]
		public void AddUniqueColumn()
		{
			provider.AddColumn("TestTwo", new Column("Test", DbType.String, 50, ColumnProperty.Unique));
		}

		[Test]
		public void CanAddForeignKey()
		{
			AddForeignKey();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "FK_Test_TestTwo"));
		}

		[Test]
		public void CanAddForeignKeyWithCommonAction()
		{
			AddForeignKeyWithCommonAction();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "FK_Test_TestTwo"));
		}

		[Test]
		public virtual void CanAddForeignKeyWithDifferentActions()
		{
			AddForeignKeyOnDeleteCascadeOnUpdateCascade();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "FK_Test_TestTwo"));
		}

		[Test]
		public virtual void CanAddUniqueConstraint()
		{
			AddUniqueConstraint();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "UN_Test_TestTwo"));
		}

		[Test]
		public virtual void CanAddMultipleUniqueConstraint()
		{
			AddMultipleUniqueConstraint();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "UN_Test_TestTwo"));
		}

		[Test]
		public virtual void CanAddCheckConstraint()
		{
			AddCheckConstraint();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "CK_TestTwo_TestId"));
		}

		[Test]
		public void RemoveForeignKey()
		{
			AddForeignKey();
			provider.RemoveForeignKey("TestTwo", "FK_Test_TestTwo");
			Assert.IsFalse(provider.ConstraintExists("TestTwo", "FK_Test_TestTwo"));
		}

		[Test]
		public void RemoveUniqueConstraint()
		{
			AddUniqueConstraint();
			provider.RemoveConstraint("TestTwo", "UN_Test_TestTwo");
			Assert.IsFalse(provider.ConstraintExists("TestTwo", "UN_Test_TestTwo"));
		}

		[Test]
		public virtual void RemoveCheckConstraint()
		{
			AddCheckConstraint();
			provider.RemoveConstraint("TestTwo", "CK_TestTwo_TestId");
			Assert.IsFalse(provider.ConstraintExists("TestTwo", "CK_TestTwo_TestId"));
		}

		[Test]
		public void RemoveUnexistingForeignKey()
		{
			AddForeignKey();
			provider.RemoveForeignKey("abc", "FK_Test_TestTwo");
			provider.RemoveForeignKey("abc", "abc");
			provider.RemoveForeignKey("Test", "abc");
		}

		[Test]
		public void ConstraintExist()
		{
			AddForeignKey();
			Assert.IsTrue(provider.ConstraintExists("TestTwo", "FK_Test_TestTwo"));
			Assert.IsFalse(provider.ConstraintExists("abc", "abc"));
		}


		[Test]
		public void AddTableWithCompoundPrimaryKey()
		{
			provider.AddTable("Test",
							  new Column("PersonId", DbType.Int32, ColumnProperty.PrimaryKey),
							  new Column("AddressId", DbType.Int32, ColumnProperty.PrimaryKey)
				);
			Assert.IsTrue(provider.TableExists("Test"));
			Assert.IsTrue(provider.ConstraintExists("Test", "PK_Test"));
		}

		[Test]
		public void AddForeignKeyWithDeleteCascade()
		{
			AddTableWithPrimaryKey();

			provider.AddForeignKey("FK", "TestTwo", "TestId", "Test", "Id", ForeignKeyConstraint.Cascade);

			provider.Insert("Test", new[] { "Id", "Name" }, new[] { "42", "aaa" });
			provider.Insert("TestTwo", new[] { "Id", "TestId" }, new[] { "1", "42" });
			provider.Delete("Test", provider.QuoteName("Id") + " = 42");

			string sql = "SELECT count(*) FROM {0} WHERE {1} = {2}".FormatWith(
				provider.QuoteName("TestTwo"), provider.QuoteName("TestId"), 42);

			object count = provider.ExecuteScalar(sql);
			Assert.AreEqual(0, count);
		}

		[Test]
		public void AddTableWithCompoundPrimaryKeyShouldKeepNullForOtherProperties()
		{
			provider.AddTable("Test",
							  new Column("PersonId", DbType.Int32, ColumnProperty.PrimaryKey),
							  new Column("AddressId", DbType.Int32, ColumnProperty.PrimaryKey),
							  new Column("Name", DbType.String, 30, ColumnProperty.Null)
				);

			Assert.IsTrue(provider.TableExists("Test"));
			Assert.IsTrue(provider.ConstraintExists("Test", "PK_Test"));

			provider.Insert("Test",
				new[] {"PersonId", "AddressId", "Name"},
				new[] {"1", "2", null});

			string sql = provider.FormatSql("select {0:NAME} from {1:NAME}", "Name", "Test");

			using (var reader = provider.ExecuteReader(sql))
			{
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(DBNull.Value, reader[0]);
				Assert.IsFalse(reader.Read());
			}
		}
	}
}