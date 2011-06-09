use strict;

my $destfile = "c:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\CswAll.min.js";

unlink($destfile);

my $param = "";
$param .= extract("c:\\kiln\\nbt\\nbt\\nbtwebapp\\js");
$param .= extract("c:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\jquery");
$param .= extract("c:\\kiln\\nbt\\nbt\\nbtwebapp\\js\\jquery\\fieldtypes");

`java -jar "C:\\kiln\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

sub extract
{
	my $filelist = "";
	my $path = $_[0];
	opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
	while((my $filename = readdir(JSDIR)))
	{
		if($filename =~ /.*\.js$/ &&
		   $filename !~ /-vsdoc/) 
		{
			printf("Compiling: $path\\$filename\n");
			$filelist .= "--js $path\\$filename ";
		}
	}
	closedir(JSDIR);
	return $filelist;
}

printf("Finished compiling javascript\n");