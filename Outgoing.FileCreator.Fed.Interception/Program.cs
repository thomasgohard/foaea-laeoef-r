using Outgoing.FileCreator.Fed.Interception;

//var process = new string[] { "OASBFOUT", "TRBFOUT" };
//await OutgoingFileCreatorFedInterception.RunBlockFunds(process);

//await OutgoingFileCreatorFedInterception.RunCRA();
await OutgoingFileCreatorFedInterception.RunEI();
//await OutgoingFileCreatorFedInterception.RunCPP();

Console.ReadKey();
