#coding: utf-8 

class BigTable
	attr_reader :xsize
	attr_reader :ysize
	attr_reader :zsize
	attr_accessor :data
      def initialize(x, y = 1, z = 1)
         x = [x, 256 ** 4].min
         y = [y, 256 ** 4].min
         z = [z, 256 ** 4].min
         @xsize, @ysize, @zsize = x, y, z
         @data = Array.new(x * y * z, 0)
      end
      def [](x, y = 0, z = 0)
         x = [x, @xsize].min
         y = [y, @ysize].min
         z = [z, @zsize].min
         @data[x + y * @xsize + z * @xsize * @ysize]
      end
      def []=(*args)
         x = [args[0], @xsize].min
         y = [args.size>2 ?args[1] : 0, @ysize].min
         z = [args.size>3 ?args[2] : 0, @zsize].min
         v = [args.pop, 256 ** 4].min
         @data[x + y * @xsize + z * @xsize * @ysize] = v
      end
 end
 
 class MapX

	attr_accessor :width
	attr_accessor :height
	attr_accessor :z_index
	attr_accessor :layers
	attr_accessor :tilesets
	attr_accessor :objects

	def initialize(width = 1, height = 1)
		@width = width
		@height = height
		@z_index = [0,50,75,100,150,250]
		@tilesets = [[],[],[],[],[],[]]
		@layers = BigTable.new(width, height, 9)
		@objects = {}
	end

end

class UObject
	attr_accessor :layer
	attr_accessor :sub_layer
	attr_accessor :x
	attr_accessor :y
	attr_accessor :z
	attr_accessor :tile_index
	attr_accessor :tile_id
	attr_accessor :span_x
	attr_accessor :span_y
	attr_accessor :crop_top
	attr_accessor :crop_bottom
	attr_accessor :crop_left
	attr_accessor :crop_right
end

