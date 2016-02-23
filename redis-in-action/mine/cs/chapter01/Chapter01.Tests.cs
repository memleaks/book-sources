using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using helper;

namespace RedisInAction.CH01
{
	partial class Chapter01 : Chapter
	{
		[RIATest]
		public void TestArticleFunctionality() {
			var article_id = PostArticle(connection, "username", "A title", "http://www.google.com");
			Assert.That(article_id, Is.GreaterThan(0));
			PrintLine("We posted a new article with id: {0}", article_id);
			PrintBlank();

			PrintLine("It's HASH looks like:");
			var article = connection.HashGetAll("article:" + article_id);
			Assert.That(article, Is.Not.Null.Or.Empty);
			PrintHash(article);
			PrintBlank();

			VoteArticle(connection, "other_user", "article:" + article_id);
			var vote = (int)connection.HashGet("article:" + article_id, "votes");
			Assert.That(vote, Is.GreaterThan(1));
			PrintLine("We voted for the article, it now has votes: {0}", vote);
			PrintBlank();

			PrintLine("The currently highest-scoring articles are:");
			var articles = GetArticles(connection, 1);
			Assert.That(articles, Is.Not.Null.Or.Empty);
			PrintHashList(articles);
			PrintBlank();

			AddRemoveGroup(connection, article_id, new[] { "new-group" });
			PrintLine("We added the article to a new group, other articles include:");
			articles = GetGroupArticles(connection, "new-group", 1);
			Assert.That(articles, Is.Not.Null.Or.Empty);
			PrintHashList(articles);
		}
	}
}
