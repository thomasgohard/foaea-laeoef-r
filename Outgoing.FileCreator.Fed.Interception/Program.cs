using Outgoing.FileCreator.Fed.Interception;

// args = args.Append("CPP").ToArray();

if ((args.Length == 0) || args.Contains("OAS") || args.Contains("TR"))
{
    var processList = new List<string>();

    if ((args.Length == 0) || args.Contains("OAS"))
        processList.Add("OASBFOUT");

    if ((args.Length == 0) || args.Contains("TR"))
        processList.Add("TRBFOUT");

    await OutgoingFileCreatorFedInterception.RunBlockFunds(processList.ToArray());
   // TODO: await OutgoingFileCreatorFedInterception.RunDivertFunds(processList.ToArray());
}

if ((args.Length == 0) || args.Contains("CRA"))
{
    await OutgoingFileCreatorFedInterception.RunCRA();
}

if ((args.Length == 0) || args.Contains("EI"))
{
    await OutgoingFileCreatorFedInterception.RunEI();
}

if ((args.Length == 0) || args.Contains("CPP"))
{
    await OutgoingFileCreatorFedInterception.RunCPP();
}

