var fs = require("fs");

fs.readFile('Text.in', function (err, data) {
  if (err) {
    return console.error(err);
  }
  lines = data.toString().split(/\r?\n/);

  var index = 0;
  while (index < lines.length) {
    var index = processSection(lines, index);
    return; // TODO remove
  }

});

// Constants
var LINE_TYPE_NORMAL = "";
var LINE_TYPE_OPTION = "Option";

function processSection(lines, index) {
  // Clear blank lines in front of section
  while(isBlank(lines[index])) {
    ++index;
    if (index >= lines.length) {
      return index;
    }
  }

  var isFirst = true;
  var tabCount = -1;
  var root = {type:LINE_TYPE_NORMAL, text:"", children:[]};
  var stack = [];
  var lastNode = root;
  while(!isBlank(lines[index]) && (index < lines.length)) {
    var line = lines[index];
    if (isFirst) {
      lastNode.text = line;
      isFirst = false;
      ++index;
      continue;
    }

    // Work out if it's normal text or an option
    var newNode;
    if (line.trimLeft().startsWith('-')) {
      newNode = {type:LINE_TYPE_OPTION, text:line.trimLeft().substring(1).trim(), children:[]};
    } else {
      newNode = {type:LINE_TYPE_NORMAL, text:line.trim(), children:[]};
    }
    //console.log("Ok, processing line (" + tabCount + "): " + JSON.stringify(newNode));

    // See if we've tabbed in or not
    var lineTabs = countLineTabs(line);
    if (lineTabs > tabCount) {
      stack.push(lastNode);
      //console.log("PUSH: tabCount " + tabCount + " lineTabs " + lineTabs + "---"+ JSON.stringify(stack[stack.length - 1], null, 1));
    } else if (lineTabs < tabCount) {
      for (var i = 0; i < (tabCount - lineTabs); i++)
        stack.pop();
      //console.log("POP: tabCount " + tabCount + " lineTabs " + lineTabs + "---"+ JSON.stringify(stack[stack.length - 1], null, 1));
    }
    stack[stack.length - 1].children.push(newNode);

    tabCount = lineTabs;
    lastNode = newNode;
    ++index;
  }

  console.log(JSON.stringify(root, null, 4));
  console.log("------------")

  printSection(root);

  return index;
}

function printSection(node) {
  var prefix = [];

  prefix.push(node.text);

  printNode(prefix, 1, node, false);
}

function printNode(prefix, depth, node, parentHasOpt) {
  var newParentHasOpt = false;
  if (hasOption(node.children)) {
    console.log(prefix.join("") + depth + "O@" + getOptionStr(node.children));
    newParentHasOpt = true;
  } else {
    if (!parentHasOpt) {
      if (depth-1 > 0) {
      console.log(prefix.join("") + (depth-1) + "@" + node.text);
      }
    }
  }

  var optionCount = 1;
  node.children.forEach((x) => {
    if (x.type == LINE_TYPE_OPTION) {
      prefix.push(depth + "O" + optionCount);
      ++optionCount;
    }
    printNode(prefix, depth+1, x, newParentHasOpt);
    if (x.type == LINE_TYPE_OPTION) {
      prefix.pop();
    }
  });
}

function isEmpty(str) {
    return (!str || 0 === str.length);
}

function hasOption(children) {
  for (x of children) {
    if (x.type == LINE_TYPE_OPTION) {
      return true;
    }
  }
  return false;
}

function getOptionStr(children) {
  return children.filter(x => x.type == LINE_TYPE_NORMAL)
    .map(x => x.text)
    .join("") +
    "|" +
    children.filter(x => x.type == LINE_TYPE_OPTION)
    .map(x => x.text)
    .join('|');
}

function isBlank(line) {
  if (!line) {
    return true;
  }
  if (line.trim() === "") {
    return true;
  }
  return false;
}

function countLineTabs(str) {
  var tabCount = 0;
  for (var i = 0; i < str.length; i++) {
    if (str.charAt(i) === "\t") {
      ++tabCount;
    } else {
      break;
    }
  }
  return tabCount;
}

function nodeToJson(node) {
  return JSON.stringify(node, (key,value) => {
    if (key=="parent") return undefined;
    else return value;
  });
}

function nodeToJsonIndent(node) {
  return JSON.stringify(node, (key,value) => {
    if (key=="parent") return undefined;
    else return value;
  }, 4);
}
