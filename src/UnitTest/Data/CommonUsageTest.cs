
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Definition;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.MockSql.Recorder;

using org.hanzify.llf.UnitTest.Data.Objects;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    #region Objects

    [DbTable("People")]
    class SinglePerson : DbObject
    {
        public string Name = null;
    }

    #endregion

    [TestFixture]
    public class CommonUsageTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
            StaticRecorder.ClearMessages();
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
            SinglePerson p = new SinglePerson();
            p.Name = "abc";
            Assert.AreEqual(0, p.Id);

            DbEntry.Save(p);
            Assert.IsTrue(0 != p.Id);
            SinglePerson p1 = DbEntry.GetObject<SinglePerson>(p.Id);
            Assert.AreEqual(p.Name, p1.Name);

            p.Name = "xyz";
            DbEntry.Save(p);
            Assert.AreEqual(p.Id, p1.Id);

            p1 = DbEntry.GetObject<SinglePerson>(p.Id);
            Assert.AreEqual("xyz", p1.Name);

            long id = p.Id;
            DbEntry.Delete(p);
            Assert.AreEqual(0, p.Id);
            p1 = DbEntry.GetObject<SinglePerson>(id);
            Assert.IsNull(p1);
        }

        [Test]
        public void Test2()
        {
            List<SinglePerson> l = DbEntry
                .From<SinglePerson>()
                .Where(null)
                .OrderBy("Id")
                .Range(1, 1)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0].Id);
            Assert.AreEqual("Tom", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(null)
                .OrderBy("Id")
                .Range(2, 2)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(2, l[0].Id);
            Assert.AreEqual("Jerry", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(null)
                .OrderBy("Id")
                .Range(3, 5)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(3, l[0].Id);
            Assert.AreEqual("Mike", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(null)
                .OrderBy((DESC)"Id")
                .Range(3, 5)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0].Id);
            Assert.AreEqual("Tom", l[0].Name);
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual(3, DbEntry.From<Category>().Where(null).GetCount());
            Assert.AreEqual(5, DbEntry.From<Book>().Where(null).GetCount());
            Assert.AreEqual(2, DbEntry.From<Book>().Where(CK.K["Category_Id"] == 3).GetCount());
        }

        [Test]
        public void Test4()
        {
            List<GroupByObject<long>> l = DbEntry
                .From<Book>()
                .Where(null)
                .OrderBy((DESC)DbEntry.CountColumn)
                .GroupBy<long>("Category_Id");

            Assert.AreEqual(2, l[0].Column);
            Assert.AreEqual(3, l[0].Count);

            Assert.AreEqual(3, l[1].Column);
            Assert.AreEqual(2, l[1].Count);
        }

        [Test]
        public void Test5()
        {
            IList l = DbEntry
                .From<Book>()
                .Where(null)
                .GroupBy<string>("Name");

            Assert.AreEqual(5, l.Count);

            l = DbEntry
                .From<Book>()
                .Where(CK.K["Id"] > 2)
                .GroupBy<string>("Name");

            Assert.AreEqual(3, l.Count);

            List<GroupByObject<string>> ll = DbEntry
                .From<Book>()
                .Where(CK.K["Id"] > 2)
                .OrderBy("Name")
                .GroupBy<string>("Name");

            Assert.AreEqual(3, ll.Count);
            Assert.AreEqual("Pal95", ll[0].Column);
            Assert.AreEqual("Shanghai", ll[1].Column);
            Assert.AreEqual("Wow", ll[2].Column);
        }

        [Test]
        public void TestPeopleModel()
        {
            List<PeopleModel> l = PeopleModel.FindAll();
            Assert.AreEqual(3, l.Count);
            Assert.AreEqual("Tom", l[0].Name);

            PeopleModel p = PeopleModel.FindByName("Jerry");
            Assert.AreEqual(2, p.Id);
            Assert.IsTrue(p.IsValid());

            p.Name = "llf";
            Assert.IsTrue(p.IsValid());
            p.Save();

            PeopleModel p1 = PeopleModel.FindById(2);
            Assert.AreEqual("llf", p1.Name);

            p.Delete();
            p1 = PeopleModel.FindById(2);
            Assert.IsNull(p1);

            p = PeopleModel.New();
            p.Name = "123456";
            Assert.IsFalse(p.IsValid());

            Assert.AreEqual(1, PeopleModel.CountName("Tom"));
            Assert.AreEqual(0, PeopleModel.CountName("xyz"));
        }

        [Test]
        public void TestSql()
        {
            PeopleModel p1 = DbEntry.Context.ExecuteList<PeopleModel>("Select [Id],[Name] From [People] Where [Id] = 2")[0];
            Assert.AreEqual("Jerry", p1.Name);
            p1 = DbEntry.Context.ExecuteList<PeopleModel>(new SqlStatement("Select [Name],[Id] From [People] Where [Id] = 1"))[0];
            Assert.AreEqual("Tom", p1.Name);
            p1 = PeopleModel.FindBySql("Select [Id],[Name] From [People] Where [Id] = 2")[0];
            Assert.AreEqual("Jerry", p1.Name);
            p1 = PeopleModel.FindBySql(new SqlStatement("Select [Name],[Id] From [People] Where [Id] = 3"))[0];
            Assert.AreEqual("Mike", p1.Name);
        }

        [Test]
        public void ToStringTest()
        {
            ImpPeople p = new ImpPeople();
            p.Name = "tom";
            Assert.AreEqual("{ Id = 0, Name = tom }", p.ToString());

            DArticle a = DArticle.New();
            a.Name = "long";
            Assert.AreEqual("{ Id = 0, Name = long }", a.ToString());

            ImpPCs c = new ImpPCs();
            c.Name = "HP";
            Assert.AreEqual("{ Id = 0, Name = HP, Person_Id = 0 }", c.ToString());
        }

        [Test]
        public void TestColumnCompColumn()
        {
            //WhereCondition c = CK.K["Age"] > CK.K["Count"];
            WhereCondition c = CK.K["Age"].Gt(CK.K["Count"]);
            DataParamterCollection dpc = new DataParamterCollection();
            string s = c.ToSqlText(ref dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn2()
        {
            WhereCondition c = CK.K["Age"] > CK.K["Count"];
            DataParamterCollection dpc = new DataParamterCollection();
            string s = c.ToSqlText(ref dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn3()
        {
            WhereCondition c = CK.K["Age"] > CK.K["Count"] && CK.K["Name"] == CK.K["theName"] || CK.K["Age"] <= CK.K["Num"];
            DataParamterCollection dpc = new DataParamterCollection();
            string s = c.ToSqlText(ref dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("(([Age] > [Count]) And ([Name] = [theName])) Or ([Age] <= [Num])", s);
        }

        [Test]
        public void TestGetSqlStetement()
        {
            SqlStatement sql = DbEntry.Context.GetSqlStatement("select * from User where Age > ? And Age < ?", 18, 23);
            Assert.AreEqual("select * from User where Age > @p0 And Age < @p1", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Paramters[0].Key);
            Assert.AreEqual(18, sql.Paramters[0].Value);
            Assert.AreEqual("@p1", sql.Paramters[1].Key);
            Assert.AreEqual(23, sql.Paramters[1].Value);
        }

        [Test]
        public void TestGetSqlStetement2()
        {
            SqlStatement sql = DbEntry.Context.GetSqlStatement("Select * from User where Id = ? Name Like '%?%' Age > ? And Age < ? ", 1, 18, 23);
            Assert.AreEqual("Select * from User where Id = @p0 Name Like '%?%' Age > @p1 And Age < @p2 ", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Paramters[0].Key);
            Assert.AreEqual(1, sql.Paramters[0].Value);
            Assert.AreEqual("@p1", sql.Paramters[1].Key);
            Assert.AreEqual(18, sql.Paramters[1].Value);
            Assert.AreEqual("@p2", sql.Paramters[2].Key);
            Assert.AreEqual(23, sql.Paramters[2].Value);
        }

        [Test]
        public void TestGetSqlStetementByExecuteList()
        {
            List<Person> ls = DbEntry.Context.ExecuteList<Person>("select * from [People] where Id > ? And Id < ?", 1, 3);
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Jerry", ls[0].Name);
        }
    }
}