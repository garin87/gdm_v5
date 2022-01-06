import { Directive, ElementRef, AfterViewInit, OnDestroy, Renderer2, Input, Output, EventEmitter } from '@angular/core';

@Directive({    
    selector: '[csi-closable]'
})

export class ClosableDirective implements AfterViewInit, OnDestroy {
    @Input('csi-closable') parentElement : any;   
    @Output('onClose') onClose = new EventEmitter<any>();
    spanBtn : any;    
    foreColor : string = "#1E1E1E";

    constructor( private _element: ElementRef,  
                 private _rd: Renderer2) {    }

    ngAfterViewInit(): void {
        const col = window.getComputedStyle(this._element.nativeElement, null).getPropertyValue("color");       
        if( col ) this.foreColor = col;

        this.spanBtn = this._rd.createElement("span");        
        this._rd.addClass(this.spanBtn, "close-tab-icon");        
        this._rd.listen(this.spanBtn, "click", (ev) => this.onClick(ev));        
        this._rd.appendChild( this.spanBtn, this.createSvg(this._rd));        
        this._rd.appendChild(this._element.nativeElement, this.spanBtn);  
     }   

    ngOnDestroy(){ 
        this._rd.removeChild(this._element.nativeElement, this.spanBtn);    
    }

    onClick(ev: Event){  
        this.onClose.emit(this.parentElement);    
    }

    private createSvg(r: Renderer2){       
        let s = r.createElement("svg", "svg");       
        r.setAttribute(s, "width", "16");       
        r.setAttribute(s, "height", "16");        
        r.appendChild(s, this.createSvgLine(r, true));       
        r.appendChild(s, this.createSvgLine(r, false));        
        let circle = this.createSvgCircle(r);       
        r.appendChild(s, circle);        
        r.listen(s, "mouseover", ()=>{ circle.style.display="block"; });        
        r.listen(s, "mouseout", ()=>{ circle.style.display="none"; });        
        return s;   
    }

    private createSvgLine(r: Renderer2, slash = false){  
        const c1 = "4";        
        const c2 = "12";        
        let l = r.createElement("line", "svg");        
        r.setAttribute(l, "x1", c1);        
        r.setAttribute(l, "y2", slash ? c1 : c2);        
        r.setAttribute(l, "x2", c2);        
        r.setAttribute(l, "y1", slash ? c2 : c1);       
        r.setAttribute(l, "stroke", this.foreColor);        
        r.setAttribute(l, "stroke-width", "1");       
         return l;    
    }

    private createSvgCircle(r: Renderer2) : SVGCircleElement {        
       // <circle cx="8" cy="8" r="8" style="fill:none; stroke-width:1px; stroke:white;display:none;" />       
       const rad = "8";
       let c = r.createElement("circle", "svg");        
       r.setAttribute(c, "cx", rad);        
       r.setAttribute(c, "cy", rad);        
       r.setAttribute(c, "r", rad);        
       r.setAttribute(c, "style", "fill:none; display:none; stroke-width:1px; stroke:"+this.foreColor +";");        
       return c;   
    }  
      
    } 
