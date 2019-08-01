include System
include System::Collections::Generic

file = File.open($map_path, 'rb')
$map = Marshal.load(file)
file.close

$clr_map = VAMX::MapX.new
$clr_map.Initialize($map.width, $map.height)
$clr_map.SetZIndex = System::Array[System::Int32].new($map.z_index)
6.times{|i| $map.tilesets[i].each{|s| $clr_map.Tilesets[i].Add(s.force_encoding("UTF-8").to_clr_string) } }
$clr_map.Layers.SetData = System::Array[System::Int32].new($map.layers.data)
$map.objects ||= {}
$map.objects.each{|k,v|
	a = VAMX::UObject.new
	a.ID = k
	a.Layer = v.layer
	a.SubLayer = v.sub_layer
	a.X = v.x
	a.Y = v.y
	a.Z = v.z
	a.TileIndex = v.tile_index
	a.TileID = v.tile_id
	a.SpanX = v.span_x
	a.SpanY = v.span_y
	a.CropTop = v.crop_top
	a.CropBottom = v.crop_bottom
	a.CropLeft = v.crop_left
	a.CropRight = v.crop_right
	$clr_map.Objects.Add(a)
}