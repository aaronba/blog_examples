// <copyright file="PreOperationaccountCreate.cs" company="">
// Copyright (c) 2019 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/20/2019 5:43:35 PM</date>
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace plugin_unit_test_examples.plugin_AccountCreation
{

    /// <summary>
    /// This plugin triggers on Account creation during the PreOperation stage.
    ///
    /// The function of the plugin is to take the 'Name' and 'Address1_City' fields and concatenate
    /// them into the 'new_nameandcity' field 
    /// </summary>    
    public class PreOperationaccountCreate: PluginBase
    {
        /// <summary>
        /// This property is used to identify if the code is running production or testing mode
        /// </summary>
        public bool UnitTestMode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreOperationaccountCreate"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics 365 for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public PreOperationaccountCreate(string unsecure, string secure)
            : base(typeof(PreOperationaccountCreate))
        {

            UnitTestMode = unsecure.Contains("dev");
            
        }

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            // Get the account and get the values for the field
            Entity account = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];
            string accountName = account.GetAttributeValue<string>("name");
            string city = account.GetAttributeValue<string>("address1_city");

            // If either of the fields are empty throw an Exception (this is done to demonstrate the unit test expecting an exception)
            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(city))
            {
                throw new Exception($"Account Name or City is not filled in: Name: {accountName}, City: {city}");
            }

            // Update the account attribute
            account.Attributes["new_nameandcity"] = $"{accountName} - {city}";

            // If we're running in production then we do not need to call an update, but if we're running
            // in Unit Test mode then we need to call the update method
            if (UnitTestMode)
            {
                localContext.OrganizationService.Update(account);
            }
        }
    }
}
