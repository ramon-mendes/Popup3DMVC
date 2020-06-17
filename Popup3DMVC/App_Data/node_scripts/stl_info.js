var NodeStl = require('node-stl')

var cl_original = console.log;
console.log = function(...args) { };

var filepath = process.argv[2];
var stl = NodeStl(filepath);

const data = {
    volume: stl.volume,
    x: stl.boundingBox[0],
    y: stl.boundingBox[1],
    z: stl.boundingBox[2],
};

console.log = cl_original;
console.log(JSON.stringify(data));