use strict;

my $dir = $ARGV[0];
my $destfile = "$dir\\CswAll.min.js";

unlink($destfile);

my $param = "";
$param .= extract("$dir\\js\\nbt");
$param .= extract("$dir\\js\\nbt\\actions");
$param .= extract("$dir\\js\\nbt\\controls");
$param .= extract("$dir\\js\\nbt\\fieldtypes");
$param .= extract("$dir\\js\\nbt\\node");
$param .= extract("$dir\\js\\nbt\\pagecmp");
$param .= extract("$dir\\js\\nbt\\tools");
$param .= extract("$dir\\js\\nbt\\view");

`java -jar "$dir\\..\\..\\..\\ThirdParty\\ClosureCompiler\\compiler.jar" $param --js_output_file $destfile`;

sub extract
{
    my $filelist = "";
    my $path = $_[0];
    opendir(JSDIR, $path) or die("Cannot open js directory: $path ; $!");
    while((my $filename = readdir(JSDIR)))
    {
        if($filename =~ /.*\.js$/ &&
           $filename !~ /-vsdoc/ &&
           $filename !~ /_first/ &&
           $filename !~ /_last/ &&
           $filename !~ /.min\.js/) 
        {
            printf("Compiling: $path\\$filename\n");
            $filelist .= "--js $path\\$filename ";
        }
    }
    closedir(JSDIR);
    return $filelist;
}

printf("Finished compiling javascript\n");