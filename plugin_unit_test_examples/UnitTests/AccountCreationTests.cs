using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using plugin_unit_test_examples.plugin_AccountCreation;

namespace UnitTests
{
    [TestClass]
    public class AccountCreationTests
    {
        /// <summary>
        /// Test that when a new account is created the new_nameandcity field is updated with the values
        /// from the name & address1_city fields
        /// </summary>
        [TestMethod]
        public void Field_Updated_When_Account_Created()
        {
            // Create the fake context. This allows us to setup a mocked version of Dynamics for use later in the test
            XrmFakedContext context = new XrmFakedContext();

            // Generate a GUID which will be assigned to the fake record. It is done here to allow the record to be retrieved
            // later during the test
            Guid accountGuid = Guid.NewGuid();

            // Create the entity record which will be the target. In this case I have set the name & address1_city
            // field to have a value, and assigned the new_nameandcity field to have no value
            Entity account = new Entity("account")
            {
                Id = accountGuid,
                Attributes =
                {
                    ["name"] = "Acme",
                    ["address1_city"] = "America",
                    ["new_nameandcity"] = ""
                }
            };

            // This will setup the context with the entities created. This is the set of entities we will be able to access
            // during the test. 
            context.Initialize(new List<Entity> { account });

            // Execute the plugin. I have added the MessageName & stage as this plugin executes PreOperation
            context.ExecutePluginWithTarget<PreOperationaccountCreate>(account, "Create", 20);

            // Retrieve the update account from the fake context
            Entity updatedAccount = context.GetOrganizationService()
                .Retrieve("account", accountGuid, new ColumnSet(new string[] { "new_nameandcity" }));

            // Assert that the value meets our expected value
            Assert.AreEqual("Acme - America", updatedAccount.GetAttributeValue<string>("new_nameandcity"));
        }

        /// <summary>
        /// The plugin should throw an exception if the city or name fields are not filled in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Error_Thrown_If_City_Is_Null()
        {
            // Create the fake context. This allows us to setup a mocked version of Dynamics for use later in the test
            XrmFakedContext context = new XrmFakedContext();

            // Generate a GUID which will be assigned to the fake record. It is done here to allow the record to be retrieved
            // later during the test
            Guid accountGuid = Guid.NewGuid();

            // Create an account record which has no address1_city filled in. This will be used during the plugin execution
            Entity account = new Entity("account")
            {
                Id = accountGuid,
                Attributes =
                {
                    ["name"] = "Acme",
                    ["address1_city"] = "",
                    ["new_nameandcity"] = ""
                }
            };

            // This will setup the context with the entities created. This is the set of entities we will be able to access
            // during the test. 
            context.Initialize(new List<Entity> { account });

            // Execute the plugin. I have added the MessageName & stage as this plugin executes PreOperation
            context.ExecutePluginWithTarget<PreOperationaccountCreate>(account, "Create", 20);

            // I am not asserting anything here as the plugin only needs to throw an exception. If no exception is thrown
            // then the unit test would fail
        }
    }
}
