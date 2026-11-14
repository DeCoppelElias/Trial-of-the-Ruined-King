# Version control

Git with the GitHub remote is the only version control. The repo has no Unity
Version Control workspace or package, and `.plastic/` is ignored so none gets
committed. Batch mode commands do not pass `-nographics`.

Unity Version Control was the alternative. It lost because its editor package
opens a login dialog in batch mode that hangs any run without `-nographics`,
and keeping it alongside Git would mean two histories that drift.
