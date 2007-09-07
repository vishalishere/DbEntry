
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using NUnit.Framework;

using org.hanzify.llf.UnitTest.Data.Objects;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    [TestFixture]
    public class VBHelperTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void Test1()
        {
            using (VBHelper.UsingTransaction())
            {
                Person p = new Person();
                p.Name = "uuu";
                DbEntry.Save(p);
                Person p1 = new Person();
                p1.Name = "iii";
                DbEntry.Save(p1);
                VBHelper.Commit();
            }
            Assert.AreEqual("uuu", DbEntry.GetObject<Person>(4).Name);
            Assert.AreEqual("iii", DbEntry.GetObject<Person>(5).Name);
        }

        [Test]
        public void Test2()
        {
            try
            {
                using (VBHelper.UsingTransaction())
                {
                    Person p = new Person();
                    p.Name = "uuu";
                    DbEntry.Save(p);
                    Person p1 = new Person();
                    p1.Name = "iii";
                    DbEntry.Save(p1);
                    DbEntry.Context.ExecuteNonQuery("select form form"); // raise exception for test.
                    VBHelper.Commit();
                }
            }
            catch { }
            Person p2 = DbEntry.GetObject<Person>(4);
            Assert.IsNull(p2);
            Assert.IsNull(DbEntry.GetObject<Person>(5));
        }
    }
}