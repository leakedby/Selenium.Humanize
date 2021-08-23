////var dots = [];
////for (var i = 0; i < 100; i++) {
////	var node = document.createElement("div");
////	node.className = "trail";
////	node.style.position = "absolute";
////	node.style.height = "6px";
////	node.style.width = "6px";
////	node.style.borderRadius = "3px";
////	node.style.background = "blue";
////	node.style.zIndex = "9999";
////	node.style.pointerEvents = "none";
////	document.body.appendChild(node);
////	dots.push(node);
////}
////var currentDot = 0;

////addEventListener("mousemove", function (event) {
////	var dot = dots[currentDot];
////	dot.style.left = (event.pageX - 3) + "px";
////	dot.style.top = (event.pageY - 3) + "px";
////	node.style.background = "blue";
////	node.style.height = "6px";
////	node.style.width = "6px";
////	currentDot = (currentDot + 1) % dots.length;
////});

////addEventListener("mousedown", function (event) {
////	var dot = dots[dots.length - 1];
////	dot.style.left = (event.pageX - 3) + "px";
////	dot.style.top = (event.pageY - 3) + "px";
////	node.style.background = "red";
////	node.style.height = "12px";
////	node.style.width = "12px";
////	currentDot = (currentDot + 1) % dots.length;
////});

// Load only once
if (document.getElementsByTagName('selenium-mouse-pointer').length == 0) {
    const box = document.createElement('selenium-mouse-pointer');
    const styleElement = document.createElement('style');
    styleElement.innerHTML = `
    selenium-mouse-pointer {
        pointer-events: none;
        position: fixed;
        top: 0;
        z-index: 10000;
        left: 0;
        width: 20px;
        height: 20px;
        background: rgba(0,0,0,.4);
        border: 1px solid white;
        border-radius: 10px;
        margin: -10px 0 0 -10px;
        padding: 0;
        transition: background .2s, border-radius .2s, border-color .2s;
    }
    selenium-mouse-pointer.button-1 {
        transition: none;
        background: rgba(0,0,0,0.9);
    }
    selenium-mouse-pointer.button-2 {
        transition: none;
        border-color: rgba(0,0,255,0.9);
    }
    selenium-mouse-pointer.button-3 {
        transition: none;
        border-radius: 4px;
    }
    selenium-mouse-pointer.button-4 {
        transition: none;
        border-color: rgba(255,0,0,0.9);
    }
    selenium-mouse-pointer.button-5 {
        transition: none;
        border-color: rgba(0,255,0,0.9);
    }
    `;
    document.head.appendChild(styleElement);
    document.body.appendChild(box);

    addEventListener('mousemove', event => {
        box.style.left = event.pageX - document.scrollingElement.scrollLeft + 'px';
        box.style.top = event.pageY - document.scrollingElement.scrollTop + 'px';
        updateButtons(event.buttons);
    }, true);

    addEventListener('mousedown', event => {
        updateButtons(event.buttons);
        box.classList.add('button-' + event.which);
    }, true);

    addEventListener('mouseup', event => {
        updateButtons(event.buttons);
        box.classList.remove('button-' + event.which);
    }, true);

    function updateButtons(buttons) {
        for (let i = 0; i < 5; i++)
            box.classList.toggle('button-' + i, buttons & (1 << i));
    }
}