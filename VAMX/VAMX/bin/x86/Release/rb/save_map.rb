include System
include System::Collections::Generic

$map = MapX.new($clr_map.Width, $clr_map.Height)

$map.z_index = $clr_map.ZIndex.to_a
$map.layers.data = $clr_map.Layers.Data.to_a
6.times{|i|
	arr = Array.new
	$clr_map.Tilesets[i].to_a.each{|s| arr.push(s.to_s.force_encoding("UTF-8"))}
	$map.tilesets[i] = arr
}
$clr_map.Objects.each{|a|
	b = UObject.new
	b.layer = a.layer
	b.sub_layer = a.sub_layer
	b.x = a.X
	b.y = a.Y
	b.z = a.Z
	b.tile_id = a.TileID
	b.tile_index = a.TileIndex
	b.span_x = a.SpanX
	b.span_y = a.SpanY
	b.crop_top = a.CropTop
	b.crop_bottom = a.CropBottom
	b.crop_left = a.CropLeft
	b.crop_right = a.CropRight
	$map.objects[a.ID] = b
}

file = File.open($map_path, 'wb')
Marshal.dump($map,file)
file.close
