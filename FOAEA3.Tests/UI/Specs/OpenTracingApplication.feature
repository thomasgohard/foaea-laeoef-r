Feature: TracingApplication
	In order to work with Part II for the FOAEAA
	As a FOAEA user
	I want to create and modify tracing applications

Scenario: Open Tracing Details Page for Existing Application
	Given I have logged in
	And Accepted the Terms of Reference
	And Selected the ON2D68 submitter
	And I have done a search for Tracing Applications
	And I see a list of Tracing applications
	When I click on an application control code link in the list
	Then the tracing details page opens
	And Close the browser

Scenario: Modify and Save Existing Tracing Application
	Given I have logged in
	And Accepted the Terms of Reference
	And Selected the ON2D68 submitter
	And I have done a search for Tracing Applications
	And I see a list of Tracing applications
	And I clicked on an application control code link in the list
	And The Tracing details page opened
	When I change the second name of the person to be located
	And I save the changes
	Then I get a confirmation that the save was successful
	And Close the browser

Scenario: Create a new Tracing Application
	Given I have logged in
	And Accepted the Terms of Reference
	And Selected the ON2D68 submitter
	And I am on the start page of FOAEA
	When I click on the T01 Quick Create button
	And I enter the key information for the new person to locate
	And I save the changes
	Then I get a confirmation that the save was successful
	And Close the browser