using Outgoing.FileCreator.Fed.Interception;

// var process = new string[] { "OASBFOUT", "TRBFOUT" };
var process = new string[] { "TRBFOUT" };
await OutgoingFileCreatorFedInterception.RunBlockFunds(process);
