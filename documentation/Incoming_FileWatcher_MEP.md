**NAME**

Incoming.FileWatcher.MEP.exe - command line tool that processes any new incoming MEP files
	
**SYNOPSIS**

`Incoming.FileWatcher.MEP.exe [province] [category]`

**DESCRIPTION**

Looks through folders (based on `FileTable` data) for the next expected cycles for MEP files. If found, then load them and call the appropriate APIs to process them.

**OPTIONS**

- `[province]` the two letter province/territory code (e.g. **ON** for Ontario) or **ALL** to include all MEPs that use FTP
- `[category]` one of: **TRACE_ONLY**, **INTERCEPTION_ONLY**, **LICENCE_ONLY** or nothing (to include all three types)